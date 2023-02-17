using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class GpsManager : MonoBehaviour
{
    [HideInInspector] public float latitude;
    [HideInInspector] public float longitude;

    public Text[] texts;

    private IEnumerator _coroutine;

    void Awake()
    {
        StartCoroutine(AskPermission());
    }

    IEnumerator AskPermission()
    {
        //handle permission
        bool _hasFineLocationPermission = Permission.HasUserAuthorizedPermission(Permission.FineLocation);

        //WE HAVE PERMISSION SO WE CAN START THE SERVICE
        if (_hasFineLocationPermission)
        {
            yield break;
        }

        PermissionCallbacks _permissionCallbacks = new PermissionCallbacks();
        //WE DON'T HAVE PERMISSION SO WE REQUEST IT AND START SERVICES ON GRANTED.
        _permissionCallbacks.PermissionGranted += s =>
        {
            InvokeRepeating("StartGPS", 1f, 2f);
        };

        _permissionCallbacks.PermissionDenied += s => 
        {
        };

        _permissionCallbacks.PermissionDeniedAndDontAskAgain += s => 
        {
            InvokeRepeating("StartGPS", 1f, 2f);
        };

        Permission.RequestUserPermission(Permission.FineLocation, _permissionCallbacks);
    }

    IEnumerator Start()
    {
        _coroutine = UpdateGPS();

        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }


        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            longitude = Input.location.lastData.longitude;
            latitude = Input.location.lastData.latitude;
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator UpdateGPS()
    {
        float timeForUpdate = 3f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(timeForUpdate);

        while (true)
        {
            texts[0].text = "Latitude = " + latitude.ToString();
            texts[1].text = "Longitude = " + longitude.ToString();
            longitude = Input.location.lastData.longitude;
            latitude = Input.location.lastData.latitude;
            yield return updateTime;
        }
    }

    void StopGPS()
    {
        Input.location.Stop();
        StopCoroutine(_coroutine);
    }

    void OnDisable()
    {
        StopGPS();
    }

    public float DistanceBetween2Points(float latitude1, float latitude2, float longitude1, float longitude2)
    {
        float distance = Mathf.Acos(Mathf.Sin(latitude1) * Mathf.Sin(latitude2) + Mathf.Cos(latitude1) * Mathf.Cos(longitude2 - longitude1)) * 6371;
        return distance;
    }

}

