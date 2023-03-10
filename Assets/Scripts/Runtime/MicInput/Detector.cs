// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
namespace PitchDetector
{
	public class Detector
	{
		private PitchTracker _tracker;
		public Detector ()
		{
			_tracker = new PitchTracker();
			_tracker.PitchRecordHistorySize = 20;
			_tracker.RecordPitchRecords = true;
		}

		public void setSampleRate(int samplerate) {
			_tracker.SampleRate = samplerate;
		}

		public void DetectPitch(float[] inBuffer) {
			_tracker.ProcessBuffer (inBuffer);
		}

		public int findModa(int count) {
			int moda = 0;
			int veces = 0;
			count = (count > _tracker.PitchRecordHistorySize) ? _tracker.PitchRecordHistorySize : count;
			for (int i=_tracker.PitchRecordHistorySize-count; i<_tracker.PitchRecordHistorySize; i++) {
				PitchTracker.PitchRecord rec=(PitchTracker.PitchRecord)_tracker.PitchRecords[i];
				if(repetitions(i, count)>veces)
					moda=rec.MidiNote;
			}
			return moda;
		}

		public int lastMidiNote (int buffer=0) {
			return _tracker.CurrentPitchRecord.MidiNote;
		}

		public float lastMidiNotePrecise (int buffer=0) {
			return (float)_tracker.CurrentPitchRecord.MidiNote + ((float)_tracker.CurrentPitchRecord.MidiCents/100f);
		}

		public float lastFrequency(int buffer=0) {
			return _tracker.CurrentPitchRecord.Pitch;
		}

		public string lastNote(int buffer=0) {
			return PitchDsp.GetNoteName (_tracker.CurrentPitchRecord.MidiNote,true,true);
		}

		public string midiNoteToString(int note) {
			return PitchDsp.GetNoteName (note,true,true);
		}

		int repetitions(int element, int count) {
			int rep = 0;
			PitchTracker.PitchRecord refer=(PitchTracker.PitchRecord)_tracker.PitchRecords[element];
			int tester=refer.MidiNote;
			for (int i=_tracker.PitchRecordHistorySize-count; i<_tracker.PitchRecordHistorySize; i++) {
				PitchTracker.PitchRecord rec=(PitchTracker.PitchRecord)_tracker.PitchRecords[i];
				if(rec.MidiNote==tester)
					rep++;
			}
			return rep;
		}
	}
}

