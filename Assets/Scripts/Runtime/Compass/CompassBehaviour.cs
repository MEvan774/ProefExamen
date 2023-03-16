using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CompassBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform _mapRect;
    [SerializeField] private RectTransform _mapMovementRect;

    [SerializeField] private Transform _mapPivot;

    private bool _startTracking = false;

    private float _angleRef;

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();//this line and the line above enables the input for the compass

        StartCoroutine(InitializeCompass());
    }

    void Update()
    {
        if (_startTracking)
        {
            float angle = Mathf.SmoothDampAngle(_mapPivot.eulerAngles.z, Input.compass.magneticHeading, ref _angleRef, 0.5f);//Smooths out the choppy movements of the compass
            _mapPivot.rotation = Quaternion.Euler(0, 0, angle);//points the compass to north
        }
    }

    IEnumerator InitializeCompass()//starts compass
    {
        yield return new WaitForSeconds(1f);
        _startTracking |= Input.compass.enabled;
    }
}