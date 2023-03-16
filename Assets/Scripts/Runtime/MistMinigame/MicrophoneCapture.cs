using System.Collections;
using UnityEngine;
using UnityEngine.Android;
[RequireComponent(typeof(AudioSource))]
public class MicrophoneCapture : MonoBehaviour
{
    public float sensitivity = 100;
    public float loudness = 0;
    private AudioSource _source;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = Microphone.Start(null, true, 10, 44100);
        _source.loop = true;
        _source.volume = 0.001f;
        while (!(Microphone.GetPosition(null) > 0)) { }
        _source.Play();
    }

    void Update()
    {
        loudness = GetAvgVol() * sensitivity;
    }

    float GetAvgVol()
    {
        float[] data = new float[64];
        float a = 0;
        _source.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 64f;
    }
}