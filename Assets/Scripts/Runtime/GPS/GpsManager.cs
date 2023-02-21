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

    //Function uses a double because casting to float is a bit innefficient
    public static double Distance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Mathf.Sin((float)(dLat / 2)) * Mathf.Sin((float)(dLat / 2)) +
                Mathf.Cos((float)ToRadians(lat1)) * Mathf.Cos((float)ToRadians(lat2)) *
                Mathf.Sin((float)(dLon / 2)) * Mathf.Sin((float)(dLon / 2));

        var c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        var distance = R * c;

        return distance;
    }

    public static double ToRadians(double degrees)
    {
        //degrees converted to radians
        return degrees * Mathf.PI / 180;
    }

    private void Update()
    {
        Debug.Log(Distance(52.35989600952385, 4.8004249229028995,52.37161765521246,4.8963318838090375));
    }
}

