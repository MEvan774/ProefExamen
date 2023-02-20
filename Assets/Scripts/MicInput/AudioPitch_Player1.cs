using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using PitchDetector;

public class AudioPitch_Player1 : MonoBehaviour {

	public string SelectedDevice { get; private set; }	//Mic selected
    public string SelectedMic;

	public float CurrentPublicAmplitude;
	public float MinVolumeDB = -30f;                        //Min volume in bd needed to start detection ----> was originally -17f

	public static int CurrentPitch;
	public int MicInput;
	public int CurrentPublicPitch;
	public int CumulativeDetection = 5;                 //Number of consecutive detections used to determine current note
	public int PitchTimeInterval = 100;


	private AudioSource _audioSource;
	private Detector _pitchDetector;                        //Pitch detector object

	private float[] _clipDatas;
	private float[] _data;                                      //Sound samples data
	private float _refValue = 0.1f;                         // RMS value for 0 dB//Current buffer pointer

	private int [] _madeDetections;                             //Detections buffer
	private int _minFreq, _maxFreq;                         //Max and min frequencies window
	private int _maxDetectionsAllowed = 50;				//Max buffer size
	private int _detectionPointer = 0;
	private int _startMidiNote = 35; 						//Lowst midi printable in score
	private int _endMidiNote = 86;                      //Lowst midi printable in score

	private bool _isDoingClean = false;                     // Flag to reset AudioClip data (prevent delay)

	//si do  do#  re  re#  mi  fa  fa# sol sol#  la  la#
	private int[] _notePositions = new int[60] {0,  1,  -1,  2,  -2,  3,  4,  -4, 5,   -5, 6,   -6,
									   7,  8,  -8,  9,  -9, 10, 11, -11, 12, -12, 13, -13,
									  14, 15, -15, 16, -16, 17, 18, -18, 19, -19, 20, -20,
									  21, 22, -22, 23, -23, 24, 25, -25, 26, -26, 27, -27,
									  28, 29, -29, 30, -30, 31, 32, -32, 33, -33, 34, -34};
	//There is enough positions...



	void Awake() {
		_pitchDetector = new Detector();
		_pitchDetector.setSampleRate(AudioSettings.outputSampleRate);
        _audioSource = GetComponent<AudioSource>();
    }



	void Start () {
		GetMic();

		//InvokeRepeating("ListenAudioInput", 0.01f, 0.018f);

		StartCoroutine(RepeatUnscaledTime());
	}

	public void GetMic()
    {
		SelectedDevice = Microphone.devices[MicInput].ToString();
		SelectedMic = SelectedDevice;
		GetMicCaps();

		//Estimates bufer len, based on pitchTimeInterval value
		int bufferLen = (int)Mathf.Round(AudioSettings.outputSampleRate * PitchTimeInterval / 1000f);
		_data = new float[bufferLen];

		_madeDetections = new int[_maxDetectionsAllowed]; //Allocates detection buffer
		setUptMic();

		StartCoroutine(RepeatUnscaledTime());
	}

	IEnumerator RepeatUnscaledTime()
	{
		float elapsedTime = 0;

		while (elapsedTime <= 0.018f)
		{
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}

		ListenAudioInput();
		StartCoroutine(RepeatUnscaledTime());
	}

	void ListenAudioInput()
    {
		_audioSource.GetOutputData(_data, 0);
		float sum = 0f;
		for (int i = 0; i < _data.Length; i++)
			sum += _data[i] * _data[i];
		float rmsValue = Mathf.Sqrt(sum / _data.Length);
		float dbValue = 30f * Mathf.Log10(rmsValue / _refValue);
		CurrentPublicAmplitude = dbValue;

		if (dbValue < MinVolumeDB)
			return;

		_pitchDetector.DetectPitch(_data);
		int midiant = _pitchDetector.lastMidiNote();
		int midi = findMode();
		CurrentPitch = midi - _startMidiNote;
		CurrentPublicPitch = CurrentPitch;
		_madeDetections[_detectionPointer++] = midiant;
		_detectionPointer %= CumulativeDetection;

		if (_audioSource.time >= 9.0f && _isDoingClean == true)
		{
			CleanClip();
			_isDoingClean = false;
		}
		if (_audioSource.time >= 5.0f)
			_isDoingClean = true;
	}

	int notePosition(int note) {
		int arrayIndex = note - _startMidiNote;
		if (arrayIndex < 0)
			arrayIndex = 0; //this is a super contrabass man!!!
		if (arrayIndex > (_endMidiNote - _startMidiNote))
			arrayIndex = (_endMidiNote - _startMidiNote); //This is a megasoprano girl!!

		return _notePositions [arrayIndex];
	}

	private void CleanClip()
	{
		_clipDatas = new float[GetComponent<AudioSource>().clip.samples * GetComponent<AudioSource>().clip.channels];
		GetComponent<AudioSource>().clip.GetData(_clipDatas, 0 );
		for(int i = 0; i < _clipDatas.Length; ++i)
			_clipDatas[i] = _clipDatas[i] * 0.0f;

		GetComponent<AudioSource>().clip.SetData(_clipDatas, 0 );
	}


	void setUptMic() {
		GetComponent<AudioSource>().volume = 1f;
		GetComponent<AudioSource>().clip = null;
		GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
		GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it
		StartMicrophone();
	}

	public void GetMicCaps () {
		Microphone.GetDeviceCaps(SelectedDevice, out _minFreq, out _maxFreq);//Gets the frequency of the device
		if ((_minFreq + _maxFreq) == 0)
			_maxFreq = 44100;
	}

	public void StartMicrophone () {
		GetComponent<AudioSource>().clip = Microphone.Start(SelectedDevice, true, 10, _maxFreq);//Starts recording
		while (!(Microphone.GetPosition(SelectedDevice) > 0)){} // Wait until the recording has started
		GetComponent<AudioSource>().Play(); // Play the audio source!
	}

	public void StopMicrophone () {
		GetComponent<AudioSource>().Stop();//Stops the audio
		Microphone.End(SelectedDevice);//Stops the recording of the device
	}

	int repetitions(int element) {
		int rep = 0;
		int tester= _madeDetections[element];
		for (int i=0; i< CumulativeDetection; i++) {
			if(_madeDetections[i]==tester)
				rep++;
		}
		return rep;
	}

	public int findMode() {
		CumulativeDetection = (CumulativeDetection >= _maxDetectionsAllowed) ? _maxDetectionsAllowed : CumulativeDetection;
		int moda = 0;
		int veces = 0;
		for (int i=0; i < CumulativeDetection; i++) {
			if(repetitions(i)>veces)
				moda= _madeDetections[i];
		}
		return moda;
	}
}
