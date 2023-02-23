// CREATED BY PEERPLAY
// WWW.PEERPLAY.NL
// v1.8

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {

	public bool IsListeningToAudioListener;

	private AudioSource _audioSource;

	//FFT values
	public static float[] FreqBands = new float[8];

	private float[] _samplesLeft = new float[512];
	private float[] _samplesRight = new float[512];

	private float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];
	private float[] _freqBandHighest = new float[8];

	//audio band values
	[HideInInspector]
	public static float[] AudioBands, AudioBandBuffers;


	//Amplitude variables
	[HideInInspector]
	public static float Amplitude, AmplitudeBuffer;
	private float _amplitudeHighest;

	//audio profile
	public float AudioProfile;

	//stereo channels
	public enum Channel {Stereo, Left, Right};
	public Channel channel = new Channel ();



	public static bool  IsResettingAudioProfile;




	// void Awake()
	// {
	// 	DontDestroyOnLoad (this.gameObject);
	// }

	// Use this for initialization
	void Start () {
		AudioBands = new float[8];
		AudioBandBuffers = new float[8];


		_audioSource = GetComponent<AudioSource> ();
		AudioProfiler(AudioProfile);
	}

	// Update is called once per frame
	void Update () {
		if (_audioSource.clip != null) {
			GetSpectrumAudioSource ();
			MakeFrequencyBands ();

			BandBuffer ();

			CreateAudioBands ();

			GetAmplitude ();

		}

	}


	public void AudioProfiler(float audioProfile)
	{
		for (int i = 0; i < 8; i++) {
			_freqBandHighest [i] = audioProfile;
		}
	}

	void GetAmplitude()
	{
		float _CurrentAmplitude = 0;
		float _CurrentAmplitudeBuffer = 0;
		for (int i = 0; i < 8; i++) {
			_CurrentAmplitude += AudioBands[i];
			_CurrentAmplitudeBuffer += AudioBandBuffers[i];
		}
		if (_CurrentAmplitude > _amplitudeHighest) {
			_amplitudeHighest = _CurrentAmplitude;
		}
		Amplitude = _CurrentAmplitude / _amplitudeHighest;
		AmplitudeBuffer = _CurrentAmplitudeBuffer / _amplitudeHighest;
	}

	void CreateAudioBands()
	{
		for (int i = 0; i < 8; i++)
		{
			if (FreqBands [i] > _freqBandHighest [i]) {
				_freqBandHighest [i] = FreqBands [i];
			}
			AudioBands[i] = Mathf.Clamp((FreqBands [i] / _freqBandHighest [i]), 0, 1);
			AudioBandBuffers[i] = Mathf.Clamp((_bandBuffer [i] / _freqBandHighest [i]), 0, 1);
		}
	}



	void GetSpectrumAudioSource()
	{
		if (IsListeningToAudioListener) {
			AudioListener.GetSpectrumData (_samplesLeft, 0, FFTWindow.Hanning);
			AudioListener.GetSpectrumData (_samplesRight, 1, FFTWindow.Hanning);
		}
		if (!IsListeningToAudioListener) {
			_audioSource.GetSpectrumData (_samplesLeft, 0, FFTWindow.Hanning);
			_audioSource.GetSpectrumData (_samplesRight, 1, FFTWindow.Hanning);
		}
	}


	void BandBuffer()
	{
		for (int g = 0; g < 8; ++g) {
			if (FreqBands [g] > _bandBuffer [g]) {
				_bandBuffer [g] = FreqBands [g];
				_bufferDecrease [g] = 0.005f;
			}

			if ((FreqBands [g] < _bandBuffer [g]) && (FreqBands [g] > 0)) {
				_bandBuffer[g] -= _bufferDecrease [g];
				_bufferDecrease [g] *= 1.2f;
			}

		}
	}



	void MakeFrequencyBands()
	{
		int count = 0;

		for (int i = 0; i < 8; i++) {


			float average = 0;
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

			if (i == 7) {
				sampleCount += 2;
			}
			for (int j = 0; j < sampleCount; j++) {
				if (channel == Channel.Stereo) {
					average += (_samplesLeft [count] + _samplesRight [count]) * (count + 1);
				}
				if (channel == Channel.Left) {
					average += _samplesLeft [count] * (count + 1);
				}
				if (channel == Channel.Right) {
					average += _samplesRight [count] * (count + 1);
				}
				count++;

			}

			average /= count;

			FreqBands [i] = average * 10;

		}
	}

}
