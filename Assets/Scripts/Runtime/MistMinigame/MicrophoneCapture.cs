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
        StartCoroutine(AskPermission());

        InvokeRepeating("Loudness", 1.1f, 1f);//using invoke instead of update to avoid performance issues
    }

    void Loudness()//gives loudness
    {
        loudness = GetAvgVol() * sensitivity;
        Debug.Log(-loudness);
    }

    float GetAvgVol()//calculates averege volume
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