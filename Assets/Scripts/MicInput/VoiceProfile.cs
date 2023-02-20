using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceProfile : MonoBehaviour {
    public static int VoiceStateCurrent; // 0 = silence, 1 = talking, 2 = shouting
    public static int VoicePitch; // 0 = ultra low bass, 51 = super high soprano
    public static int PitchProfileMin, PitchProfileMax;
    public static int VoicePitchCurrent; // 0 = low pitch, 1 = normal pitch, 2 = high pitch

    // public float _voiceLength;
    public static float SilenceProfile, TalkProfile;
    public static float AmplitudeHighest, AmplitudeCurrent, AmplitudeCurrentBuffer;

    // public float _SilenceProfileBasedOnTalkValue = 0.05f;
    //  public float _ShoutingProfileBasedOnTalkValue = 1.2f;
    public static bool IsProfileSet;
    private float _talkTime, _shoutTime;

    public static float AmplitudeSilence;
    public float SilenceAmplitude;


    // void Awake()
    // {
    //     DontDestroyOnLoad(transform.gameObject);
    // }
    // Use this for initialization
    void Start () {
        AmplitudeSilence = SilenceAmplitude;
        IsProfileSet = true;

    }

	// Update is called once per frame
    public void GetAudioProfile()
    {
        if (AmplitudeCurrent > AmplitudeHighest)
        {
            AmplitudeHighest = AmplitudeCurrent;
        }
    }

    public void SetProfile()
    {
        //   _silenceProfile = _SilenceProfileBasedOnTalkValue;
        //  _talkProfile = _ShoutingProfileBasedOnTalkValue;
        IsProfileSet = true;
    }

    bool _previouslyShout;

    void Update () {
        if (IsProfileSet)
        {
            //Amplitude
            AmplitudeCurrent = 0;
            for (int i = 0; i < 8; i++)
            {
                AmplitudeCurrent += AudioPeer.FreqBands[i];
            }

            //Buffer
            if (AmplitudeCurrent > AmplitudeCurrentBuffer)
            {
                AmplitudeCurrentBuffer = AmplitudeCurrent;
            }
            if (AmplitudeCurrent < AmplitudeCurrentBuffer)
            {
                AmplitudeCurrentBuffer *= 0.90f;
            }

            //Pitch
            VoicePitch = Mathf.Clamp (AudioPitch_Player1.CurrentPitch, 0, 51);



            if (AmplitudeCurrent < SilenceProfile)
            {
                _talkTime = 0;
                _shoutTime = 0;
                VoiceStateCurrent = 0;
                _previouslyShout = false;
            }
            else if ((AmplitudeCurrent >= SilenceProfile) && (AmplitudeCurrent < TalkProfile))
            {
                if (!_previouslyShout)
                {
                    _talkTime += Time.deltaTime;
                    _shoutTime = 0;
                    VoiceStateCurrent = 1;
                }
            }
            else if (AmplitudeCurrent > TalkProfile)
            {
                _shoutTime += Time.deltaTime;
                _talkTime = 0;
                VoiceStateCurrent = 2;
                _previouslyShout = true;
            }

            // pitch
            if (VoicePitch < PitchProfileMin)
            {
                VoicePitchCurrent = 0;
            }
            if ((VoicePitch >= PitchProfileMin) && (VoicePitch <= PitchProfileMax))
            {
                VoicePitchCurrent = 1;
            }
            if (VoicePitch > PitchProfileMax)
            {
                VoicePitchCurrent = 2;
            }

        }
    }


}
