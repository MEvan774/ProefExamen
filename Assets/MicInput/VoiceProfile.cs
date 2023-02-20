using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceProfile : MonoBehaviour {
    public static int _voiceStateCurrent; // 0 = silence, 1 = talking, 2 = shouting
    public static int _voicePitch; // 0 = ultra low bass, 51 = super high soprano
    public static int _pitchProfileMin, _pitchProfileMax;
    public static int _voicePitchCurrent; // 0 = low pitch, 1 = normal pitch, 2 = high pitch
   // public float _voiceLength;
    public static float _silenceProfile, _talkProfile;
    public static float _amplitudeHighest, _amplitudeCurrent, _amplitudeCurrentBuffer;

   // public float _SilenceProfileBasedOnTalkValue = 0.05f;
  //  public float _ShoutingProfileBasedOnTalkValue = 1.2f;
    public static bool _profileSet;
    private float _talkTime, _shoutTime;

    public static float _amplitudeSilence;
    public float _silenceAmplitude;


    // void Awake()
    // {
    //     DontDestroyOnLoad(transform.gameObject);
    // }
    // Use this for initialization
    void Start () {
        _amplitudeSilence = _silenceAmplitude;
        _profileSet = true;

    }

	// Update is called once per frame
    public void GetAudioProfile()
    {
        if (_amplitudeCurrent > _amplitudeHighest)
        {
            _amplitudeHighest = _amplitudeCurrent;
        }
    }

    public void SetProfile()
    {
     //   _silenceProfile = _SilenceProfileBasedOnTalkValue;
      //  _talkProfile = _ShoutingProfileBasedOnTalkValue;
        _profileSet = true;
    }

    bool _previouslyShout;

    void Update () {
        if (_profileSet)
        {
            //Amplitude
            _amplitudeCurrent = 0;
            for (int i = 0; i < 8; i++)
            {
                _amplitudeCurrent += AudioPeer.FreqBands[i];
            }

            //Buffer
            if (_amplitudeCurrent > _amplitudeCurrentBuffer)
            {
                _amplitudeCurrentBuffer = _amplitudeCurrent;
            }
            if (_amplitudeCurrent < _amplitudeCurrentBuffer)
            {
                _amplitudeCurrentBuffer *= 0.90f;
            }

            //Pitch
            _voicePitch = Mathf.Clamp (AudioPitch._currentPitch, 0, 51);



            if (_amplitudeCurrent < _silenceProfile)
            {
                _talkTime = 0;
                _shoutTime = 0;
                _voiceStateCurrent = 0;
                _previouslyShout = false;
            }
            else if ((_amplitudeCurrent >= _silenceProfile) && (_amplitudeCurrent < _talkProfile))
            {
                if (!_previouslyShout)
                {
                    _talkTime += Time.deltaTime;
                    _shoutTime = 0;
                    _voiceStateCurrent = 1;
                }
            }
            else if (_amplitudeCurrent > _talkProfile)
            {
                _shoutTime += Time.deltaTime;
                _talkTime = 0;
                _voiceStateCurrent = 2;
                _previouslyShout = true;
            }

            // pitch
            if (_voicePitch < _pitchProfileMin)
            {
                _voicePitchCurrent = 0;
            }
            if ((_voicePitch >= _pitchProfileMin) && (_voicePitch <= _pitchProfileMax))
            {
                _voicePitchCurrent = 1;
            }
            if (_voicePitch > _pitchProfileMax)
            {
                _voicePitchCurrent = 2;
            }

        }
    }


}
