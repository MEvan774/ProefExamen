using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Profiling;
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
    }
    void Update()
    {

        loudness = GetAvgVol() * sensitivity;
        Debug.Log(loudness);

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
    IEnumerator AskPermission()
    {
        //handle permission
        bool _hasFMicrophonePermission = Permission.HasUserAuthorizedPermission(Permission.Microphone);

        //WE HAVE PERMISSION SO WE CAN START THE SERVICE
        if (_hasFMicrophonePermission)
        {
            yield break;
        }

        PermissionCallbacks _permissionCallbacks = new PermissionCallbacks();
        //WE DON'T HAVE PERMISSION SO WE REQUEST IT AND START SERVICES ON GRANTED.
        _permissionCallbacks.PermissionGranted += s => { };

        _permissionCallbacks.PermissionDenied += s => { };

        _permissionCallbacks.PermissionDeniedAndDontAskAgain += s => { };

        Permission.RequestUserPermission(Permission.Microphone, _permissionCallbacks);
    }
}