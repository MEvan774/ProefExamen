using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace PitchDetector
{
    /// <summary>
    /// Tracks pitch
    /// </summary>
    class PitchTracker
    {
        private const int _kOctaveSteps = 96;
        private const float _kMinFreq = 50.0f;               // A1, Midi note 33, 55.0Hz
        private const float _kMaxFreq = 1600.0f;             // A#6. Midi note 92
        private const float _kDetectOverlapSec = 0.005f;
        private const float _kMaxOctaveSecRate = 10.0f;

        private const float _kAvgOffset = 0.005f;	        // time offset between pitch averaging values
        private const int _kAvgCount = 1;			        // number of average pitch samples to take
        private const float _kCircularBufSaveTime = 1.0f;    // Amount of samples to store in the history buffer

        private PitchDsp _dsp;
        private CircularBuffer<float> _circularBufferLow;
        private CircularBuffer<float> _circularBufferHigh;
        private double _sampleRate;
        private float _detectLevelThreshold = 0.01f;       // -40dB
        private int _pitchRecordsPerSecond = 50;           // default is 50, or one record every 20ms

        private float[] _pitchBufLows;
        private float[] _pitchBufHighs;
        private int _pitchBufSize;
        private int _samplesPerPitchBlock;
        private int _curPitchIndex;
        private long _curPitchSamplePos;

        private int _detectOverlapSamples;
        private float _maxOverlapDiff;

        private bool _isRecordingPitchRecords;
        private int _pitchRecordHistorySize;
        private List<PitchRecord> _pitchRecords = new List<PitchRecord>();
        private PitchRecord _curPitchRecord = new PitchRecord();

        private IIRFilter _iirFilterLoLo;
        private IIRFilter _iirFilterLoHi;
        private IIRFilter _iirFilterHiLo;
        private IIRFilter _iirFilterHiHi;

        public delegate void PitchDetectedHandler(PitchTracker sender, PitchRecord pitchRecord);
        public event PitchDetectedHandler PitchDetected; 

        /// <summary>
        /// Constructor
        /// </summary>
        public PitchTracker()
        {

        }

        /// <summary>
        /// Set the sample rate
        /// </summary>
        public double SampleRate
        {
            set
            {
                if (_sampleRate == value)
                    return;

                _sampleRate = value;
                Setup();
            }
        }

        /// <summary>
        /// Set the detect level threshold, The value must be between 0.0001f and 1.0f (-80 dB to 0 dB)
        /// </summary>
        public float DetectLevelThreshold
        {
            set
            {
                var newValue = Math.Max(0.0001f, Math.Min(1.0f, value));

                if (_detectLevelThreshold == newValue)
                    return;

                _detectLevelThreshold = newValue;
                Setup();
            }
        }

        /// <summary>
        /// Return the samples per pitch block
        /// </summary>
        public int SamplesPerPitchBlock
        {
            get { return _samplesPerPitchBlock; }
        }

        /// <summary>
        /// Get or set the number of pitch records per second (default is 50, or one record every 20ms)
        /// </summary>
        public int PitchRecordsPerSecond
        {
            get { return _pitchRecordsPerSecond; }
            set
            {
                _pitchRecordsPerSecond = Math.Max(1, Math.Min(100, value));
                Setup(); 
            }
        }

        /// <summary>
        /// Get or set whether pitch records should be recorded into a history buffer
        /// </summary>
        public bool RecordPitchRecords
        {
            get { return _isRecordingPitchRecords; }
            set
            {
                if (_isRecordingPitchRecords == value)
                    return;

                _isRecordingPitchRecords = value;

                if (!_isRecordingPitchRecords)
                    _pitchRecords = new List<PitchRecord>();
            }
        }

        /// <summary>
        /// Get or set the max number of pitch records to keep. A value of 0 means no limit.
        /// Don't leave this at 0 when RecordPitchRecords is true and this is used in a realtime
        /// application since the buffer will grow indefinately!
        /// </summary>
        public int PitchRecordHistorySize
        {
            get { return _pitchRecordHistorySize; }
            set 
            { 
                _pitchRecordHistorySize = value;

                _pitchRecords.Capacity = _pitchRecordHistorySize;
            }
        }

        /// <summary>
        /// Get the current pitch records
        /// </summary>
        public IList PitchRecords
        {
            get { return _pitchRecords.AsReadOnly(); }
        }

        /// <summary>
        /// Get the latest pitch record
        /// </summary>
        public PitchRecord CurrentPitchRecord
        {
            get { return _curPitchRecord; }
        }

        /// <summary>
        /// Get the current pitch position
        /// </summary>
        public long CurrentPitchSamplePosition
        {
            get { return _curPitchSamplePos; }
        }

        /// <summary>
        /// Get the minimum frequency that can be detected
        /// </summary>
        public static float MinDetectFrequency
        {
            get { return _kMinFreq; }
        }

        /// <summary>
        /// Get the maximum frequency that can be detected
        /// </summary>
        public static float MaxDetectFrequency
        {
            get { return _kMaxFreq; }
        }

        /// <summary>
        /// Get the frequency step
        /// </summary>
        public static double FrequencyStep
        {
            get { return Math.Pow(2.0, 1.0 / _kOctaveSteps); }
        }

        /// <summary>
        /// Get the number of samples that the detected pitch is offset from the input buffer.
        /// This is just an estimate to sync up the samples and detected pitch
        /// </summary>
        public int DetectSampleOffset
        {
            get { return (_pitchBufSize + _detectOverlapSamples) / 2; }
        }

        /// <summary>
        /// Reset the pitch tracker. Call this when the sample position is
        /// not consecutive from the previous position
        /// </summary>
        public void Reset()
        {
            _curPitchIndex = 0;
            _curPitchSamplePos = 0;
            _pitchRecords.Clear();
            _iirFilterLoLo.Reset();
            _iirFilterLoHi.Reset();
            _iirFilterHiLo.Reset();
            _iirFilterHiHi.Reset();
            _circularBufferLow.Reset();
            _circularBufferLow.Clear();
            _circularBufferHigh.Reset();
            _circularBufferHigh.Clear();
            _pitchBufLows.Clear();
            _pitchBufHighs.Clear();

            _circularBufferLow.StartPosition = -_detectOverlapSamples;
            _circularBufferLow.Available = _detectOverlapSamples;
            _circularBufferHigh.StartPosition = -_detectOverlapSamples;
            _circularBufferHigh.Available = _detectOverlapSamples;
        }

        /// <summary>
        /// Process the passed in buffer of data. During this call, the PitchDetected event will
        /// be fired zero or more times, depending how many pitch records will fit in the new
        /// and previously cached buffer.
        ///
        /// This means that there is no size restriction on the buffer that is passed into ProcessBuffer.
        /// For instance, ProcessBuffer can be called with one very large buffer that contains all of the
        /// audio to be processed (many PitchDetected events will be fired), or just a small buffer at
        /// a time which is more typical for realtime applications. In the latter case, the PitchDetected
        /// event might not be fired at all since additional calls must first be made to accumulate enough
        /// data do another pitch detect operation.
        /// </summary>
        /// <param name="inBuffer">Input buffer. Samples must be in the range -1.0 to 1.0</param>
        /// <param name="sampleCount">Number of samples to process. Zero means all samples in the buffer</param>
        public void ProcessBuffer(float[] inBuffer, int sampleCount = 0)
        {
            if (inBuffer == null)
                throw new ArgumentNullException("inBuffer", "Input buffer cannot be null");

            var samplesProcessed = 0;
            var srcLength = sampleCount == 0 ? inBuffer.Length : Math.Min(sampleCount, inBuffer.Length);

            while (samplesProcessed < srcLength)
            {
                int frameCount = Math.Min(srcLength - samplesProcessed, _pitchBufSize + _detectOverlapSamples);

                _iirFilterLoLo.FilterBuffer(inBuffer, samplesProcessed, _pitchBufLows, 0, frameCount);
                _iirFilterLoHi.FilterBuffer(_pitchBufLows, 0, _pitchBufLows, 0, frameCount);

                _iirFilterHiLo.FilterBuffer(inBuffer, samplesProcessed, _pitchBufHighs, 0, frameCount);
                _iirFilterHiHi.FilterBuffer(_pitchBufHighs, 0, _pitchBufHighs, 0, frameCount);

                _circularBufferLow.WriteBuffer(_pitchBufLows, frameCount);
                _circularBufferHigh.WriteBuffer(_pitchBufHighs, frameCount);

                // Loop while there is enough samples in the circular buffer
                while (_circularBufferLow.ReadBuffer(_pitchBufLows, _curPitchSamplePos, _pitchBufSize + _detectOverlapSamples))
                {
                    float pitch1;
                    float pitch2 = 0.0f;
                    float detectedPitch = 0.0f;

                    _circularBufferHigh.ReadBuffer(_pitchBufHighs, _curPitchSamplePos, _pitchBufSize + _detectOverlapSamples);

                    pitch1 = _dsp.DetectPitch(_pitchBufLows, _pitchBufHighs, _pitchBufSize);

                    if (pitch1 > 0.0f)
                    {
                        // Shift the buffers left by the overlapping amount
                        _pitchBufLows.Copy(_pitchBufLows, _detectOverlapSamples, 0, _pitchBufSize);
                        _pitchBufHighs.Copy(_pitchBufHighs, _detectOverlapSamples, 0, _pitchBufSize);

                        pitch2 = _dsp.DetectPitch(_pitchBufLows, _pitchBufHighs, _pitchBufSize);

                        if (pitch2 > 0.0f)
                        {
                            float fDiff = Math.Max(pitch1, pitch2) / Math.Min(pitch1, pitch2) - 1.0f;

                            if (fDiff < _maxOverlapDiff)
                                detectedPitch = (pitch1 + pitch2) * 0.5f;
                        }
                    }

                    // Log the pitch record
                    AddPitchRecord(detectedPitch);

                    _curPitchSamplePos += _samplesPerPitchBlock;
                    _curPitchIndex++;
                }

                samplesProcessed += frameCount;
            }
        }

        /// <summary>
        /// Setup
        /// </summary>
        private void Setup()
        {
            if (_sampleRate < 1.0f)
                return;

            _dsp = new PitchDsp(_sampleRate, _kMinFreq, _kMaxFreq, _detectLevelThreshold);

            _iirFilterLoLo = new IIRFilter();
            _iirFilterLoLo.Proto = IIRFilter.ProtoType.Butterworth;
            _iirFilterLoLo.Type = IIRFilter.FilterType.HP;
            _iirFilterLoLo.Order = 5;
            _iirFilterLoLo.FreqLow = 45.0f;
            _iirFilterLoLo.SampleRate = (float)_sampleRate;

            _iirFilterLoHi = new IIRFilter();
            _iirFilterLoHi.Proto = IIRFilter.ProtoType.Butterworth;
            _iirFilterLoHi.Type = IIRFilter.FilterType.LP;
            _iirFilterLoHi.Order = 5;
            _iirFilterLoHi.FreqHigh = 280.0f;
            _iirFilterLoHi.SampleRate = (float)_sampleRate;

            _iirFilterHiLo = new IIRFilter();
            _iirFilterHiLo.Proto = IIRFilter.ProtoType.Butterworth;
            _iirFilterHiLo.Type = IIRFilter.FilterType.HP;
            _iirFilterHiLo.Order = 5;
            _iirFilterHiLo.FreqLow = 45.0f;
            _iirFilterHiLo.SampleRate = (float)_sampleRate;

            _iirFilterHiHi = new IIRFilter();
            _iirFilterHiHi.Proto = IIRFilter.ProtoType.Butterworth;
            _iirFilterHiHi.Type = IIRFilter.FilterType.LP;
            _iirFilterHiHi.Order = 5;
            _iirFilterHiHi.FreqHigh = 1500.0f;
            _iirFilterHiHi.SampleRate = (float)_sampleRate;

            _detectOverlapSamples = (int)(_kDetectOverlapSec * _sampleRate);
            _maxOverlapDiff = _kMaxOctaveSecRate * _kDetectOverlapSec;

            _pitchBufSize = (int)(((1.0f / (float)_kMinFreq) * 2.0f + ((_kAvgCount - 1) * _kAvgOffset)) * _sampleRate) + 16;
            _pitchBufLows = new float[_pitchBufSize + _detectOverlapSamples];
            _pitchBufHighs = new float[_pitchBufSize + _detectOverlapSamples];
            _samplesPerPitchBlock = (int)Math.Round(_sampleRate / _pitchRecordsPerSecond); 

            _circularBufferLow = new CircularBuffer<float>((int)(_kCircularBufSaveTime * _sampleRate + 0.5f) + 10000);
            _circularBufferHigh = new CircularBuffer<float>((int)(_kCircularBufSaveTime * _sampleRate + 0.5f) + 10000);
        }

        /// <summary>
        /// The pitch was detected - add the record
        /// </summary>
        /// <param name="pitch"></param>
        private void AddPitchRecord(float pitch)
        {
            var midiNote = 0;
            var midiCents = 0;

            PitchDsp.PitchToMidiNote(pitch, out midiNote, out midiCents);

            var record = new PitchRecord();

            record.RecordIndex = _curPitchIndex;
            record.Pitch = pitch;
            record.MidiNote = midiNote;
            record.MidiCents = midiCents;

            _curPitchRecord = record;

            if (_isRecordingPitchRecords)
            {
                if (_pitchRecordHistorySize > 0 && _pitchRecords.Count >= _pitchRecordHistorySize)
                    _pitchRecords.RemoveAt(0);

                _pitchRecords.Add(record);
            }
            
            if (this.PitchDetected != null)
                this.PitchDetected(this, record);
        }

        /// <summary>
        /// Stores one record
        /// </summary>
        public struct PitchRecord
        {
            /// <summary>
            /// The index of the pitch record since the last Reset call
            /// </summary>
            public int RecordIndex { get; set; }

            /// <summary>
            /// The detected pitch
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// The detected MIDI note, or 0 for no pitch
            /// </summary>
            public int MidiNote { get; set; }

            /// <summary>
            /// The offset from the detected MIDI note in cents, from -50 to +50.
            /// </summary>
            public int MidiCents { get; set; }
        }
    }
}

