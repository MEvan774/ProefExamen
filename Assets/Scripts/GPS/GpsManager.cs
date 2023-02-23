using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using TMPro;
using System;

public class GpsManager : MonoBehaviour
{
    [HideInInspector] public float latitude;
    [HideInInspector] public float longitude;
    [HideInInspector] public float startLatitude;
    [HideInInspector] public float startLongitude;
    [HideInInspector] public Decimal distance;

    public TMP_Text[] texts;

    private IEnumerator _coroutine;

    
    void Awake()
    {
        StartCoroutine(AskPermission());

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = "DEBUG DEBUG";
        }
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
        _permissionCallbacks.PermissionGranted += s =>{};

        _permissionCallbacks.PermissionDenied += s =>{};

        _permissionCallbacks.PermissionDeniedAndDontAskAgain += s =>{};

        Permission.RequestUserPermission(Permission.FineLocation, _permissionCallbacks);
    }

    IEnumerator Start()
    {
        _coroutine = UpdateGPS();

        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 3;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            for (int i = 0; i < texts.Length; i++)
            {
            texts[i].text = "Timed out.";
            }
            yield break;
        }


        if (Input.location.status == LocationServiceStatus.Failed)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].text = "Unable to determine device location";
            }
            yield break;
        }
        else
        {
            startLongitude = Input.location.lastData.longitude;
            startLatitude = Input.location.lastData.latitude;
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator UpdateGPS()
    {
        float timeForUpdate = 3f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(timeForUpdate);

        //Store the values to a temp variables  
        decimal _distance = 0;
        double prevLongitude = 0;
        double prevLatitude = 0;

        while (true)
        {
            longitude = Input.location.lastData.longitude;
            latitude = Input.location.lastData.latitude;

            texts[0].text = "Start Latitude = " + startLatitude.ToString();
            texts[1].text = "Start Longitude = " + startLongitude.ToString();

            texts[2].text = "Latitude = " + latitude.ToString();
            texts[3].text = "Longitude = " + longitude.ToString();

            if (prevLongitude != 0 && prevLatitude != 0)
            {
                double dist = Distance(prevLatitude, prevLongitude, latitude, longitude);
                _distance += (decimal)dist;
                texts[4].text = "Distance = " + _distance.ToString() + " km";
                Debug.Log(longitude);
                Debug.Log(latitude);
                Debug.Log(dist);
                Debug.Log(_distance);
            }

            prevLongitude = longitude;
            prevLatitude = latitude;

            yield return updateTime;
        }
    }

    /*
        Function uses a double because casting to float is a bit innefficient
        The haversine formula determines the great-circle distance between two points on a sphere given their longitudes and latitudes. 
        Important in navigation, it is a special case of a more general formula in spherical trigonometry, the law of haversines, that relates the sides and angles of spherical triangles.
        //Reference for the formula https://en.wikipedia.org/wiki/Haversine_formula
    */

    public double Distance(double lat1, double lon1, double lat2, double lon2)
    {
<<<<<<< Updated upstream:Assets/Scripts/GPS/GpsManager.cs
        StopGPS();
    }

    public float DistanceBetween2Points(float latitude1, float latitude2, float longitude1, float longitude2)
    {
        float distance = Mathf.Acos(Mathf.Sin(latitude1) * Mathf.Sin(latitude2) + Mathf.Cos(latitude1) * Mathf.Cos(longitude2 - longitude1)) * 6371;
        return distance;
    }

=======
        const double radius = 6371.0; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Mathf.Sin((float)(dLat / 2)) * Mathf.Sin((float)(dLat / 2)) +
                Mathf.Cos((float)ToRadians(lat1)) * Mathf.Cos((float)ToRadians(lat2)) *
                Mathf.Sin((float)(dLon / 2)) * Mathf.Sin((float)(dLon / 2));

        var c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));

        var distance = radius * c;

        return distance;
    }

    public double ToRadians(double degrees)
    {
        //degrees converted to radians
        return degrees * Mathf.PI / 180;
    }
    private void OnDisable()
    {
        StopGPS();
    }
    public void StopGPS()
    {
        Input.location.Stop();
        StopCoroutine(_coroutine);
    }
>>>>>>> Stashed changes:Assets/Scripts/Runtime/GPS/GpsManager.cs
}

