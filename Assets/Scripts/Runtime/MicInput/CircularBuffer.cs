using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PitchDetector
{
    class CircularBuffer<T> : IDisposable
    {
        private int _bufSize;
        private int _begBufOffset;
        private int _availBuf;
        private long _startPos;   // total circular buffer position
        private T[] _buffers;

        /// <summary>
        /// Constructor
        /// </summary>
        public CircularBuffer()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bufCount"></param>
        public CircularBuffer(int bufCount)
        {
            SetSize(bufCount);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            SetSize(0);
        }

        /// <summary>
        /// Reset to the beginning of the buffer
        /// </summary>
        public void Reset()
        {
            _begBufOffset = 0;
            _availBuf = 0;
            _startPos = 0;
        }

        /// <summary>
        /// Set the buffer to the specified size
        /// </summary>
        /// <param name="newSize"></param>
        public void SetSize(int newSize)
        {
            Reset();

            if (_bufSize == newSize)
                return;

            if (_buffers != null)
                _buffers = null;

            _bufSize = newSize;

            if (_bufSize > 0)
                _buffers = new T[_bufSize];
        }

        /// <summary>
        /// Clear the buffer
        /// </summary>
        public void Clear()
        {
            Array.Clear(_buffers, 0, _buffers.Length);
        }

        /// <summary>
        /// Get or set the start position
        /// </summary>
        public long StartPosition
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        /// <summary>
        /// Get the end position
        /// </summary>
        public long EndPosition
        {
            get { return _startPos + _availBuf; }
        }

        /// <summary>
        /// Get or set the amount of avaliable space
        /// </summary>
        public int Available
        {
            get { return _availBuf; }
            set { _availBuf = Math.Min(value, _bufSize); }
        }

        /// <summary>
        /// Write data into the buffer
        /// </summary>
        /// <param name="m_pInBuffer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int WriteBuffer(T[] m_pInBuffer, int count)
        {
            count = Math.Min(count, _bufSize);

            var startPos = _availBuf != _bufSize ? _availBuf : _begBufOffset;
            var pass1Count = Math.Min(count, _bufSize - startPos);
            var pass2Count = count - pass1Count;

            PitchDsp.CopyBuffer(m_pInBuffer, 0, _buffers, startPos, pass1Count);

            if (pass2Count > 0)
                PitchDsp.CopyBuffer(m_pInBuffer, pass1Count, _buffers, 0, pass2Count);

            if (pass2Count == 0)
            {
                // did not wrap around
                if (_availBuf != _bufSize)
                    _availBuf += count;   // have never wrapped around
                else
                {
                    _begBufOffset += count;
                    _startPos += count;
                }
            }
            else
            {
                // wrapped around
                if (_availBuf != _bufSize)
                    _startPos += pass2Count;  // first time wrap-around
                else
                    _startPos += count;

                _begBufOffset = pass2Count;
                _availBuf = _bufSize;
            }

            return count;
        }

        /// <summary>
        /// Read from the buffer
        /// </summary>
        /// <param name="outBuffer"></param>
        /// <param name="startRead"></param>
        /// <param name="readCount"></param>
        /// <returns></returns>
        public bool ReadBuffer(T[] outBuffer, long startRead, int readCount)
        {
            var endRead = (int)(startRead + readCount);
            var endAvail = (int)(_startPos + _availBuf);

            if (startRead < _startPos || endRead > endAvail)
                return false;

            var startReadPos = (int)(((startRead - _startPos) + _begBufOffset) % _bufSize);
            var block1Samples = Math.Min(readCount, _bufSize - startReadPos);
            var block2Samples = readCount - block1Samples;

            PitchDsp.CopyBuffer(_buffers, startReadPos, outBuffer, 0, block1Samples);

            if (block2Samples > 0)
                PitchDsp.CopyBuffer(_buffers, 0, outBuffer, block1Samples, block2Samples);

            return true;
        }
    }
}
