using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class Checkpoint
{
    public float Latitude;
    public float Longitude;
}

public class GpsManager : MonoBehaviour
{
    [HideInInspector] public float latitude;
    [HideInInspector] public float longitude;
    [HideInInspector] public float startLatitude;
    [HideInInspector] public float startLongitude;
    [HideInInspector] public Decimal distance;

    public TMP_Text[] texts;

    private IEnumerator _coroutine;

    Checkpoint checkpoint = new Checkpoint();

    public GameObject checkPointObj;

    [SerializeField]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

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
        float timeForUpdate = 1f; //Every X seconds
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

            checkpoint.Latitude = latitude;
            checkpoint.Longitude = longitude;

            if (prevLongitude != 0 && prevLatitude != 0)
            {
                double dist = Distance(prevLatitude, prevLongitude, latitude, longitude);
                _distance += (decimal)dist;
                texts[4].text = "Distance = " + Decimal.Round(_distance, 3).ToString() + " km";
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

    public void AddCheckPoints()
    {
        checkpoints.Add(checkpoint);

        var tempCheckPointObj = Instantiate(checkPointObj, GameObject.FindGameObjectWithTag("CheckPointLayout").gameObject.transform);

        tempCheckPointObj.GetComponent<TMP_Text>().text = "CheckPoint " + checkpoints.Count + "\n" + "Latitude = " + checkpoint.Latitude.ToString() + "\n" + "Longitude = " + checkpoint.Longitude.ToString();
    }

    public void RemoveCheckPoints()
    {
        checkpoints.RemoveAt(checkpoints.Count - 1);

        Destroy(GameObject.FindGameObjectWithTag("CheckPointLayout").gameObject.transform.GetChild(checkpoints.Count).gameObject);
    }

    public void RemoveCheckPointsAtLocation()
    {

        int index = GameObject.FindGameObjectWithTag("CheckPointLayout").gameObject.transform.GetSiblingIndex();

        GameObject objToDestroyAt = GameObject.FindGameObjectWithTag("CheckPointLayout").gameObject.transform.GetChild(GameObject.FindGameObjectWithTag("CheckPointLayout").gameObject.transform.GetSiblingIndex()).gameObject; 

        checkpoints.RemoveAt(index);

        Destroy(objToDestroyAt);
    }

    public void SaveCheckPoints()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/checkpoints.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, checkpoints);
        stream.Close();

        Debug.Log("Saved Checkpoints at " + path);
    }

    public void LoadCheckPoints()
    {
        string path = Application.persistentDataPath + "/checkpoints.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<Checkpoint> loadedCheckpoints = formatter.Deserialize(stream) as List<Checkpoint>;
            stream.Close();

            checkpoints = loadedCheckpoints;

            Debug.Log("Loaded Checkpoints from " + path);
        }
        else
        {
            Debug.LogError("Checkpoints file not found at " + path);
        }
    }

    public void DeleteCheckpoints()
    {
        string path = Application.persistentDataPath + "/checkpoints.bin";
        if (File.Exists(path))
        {
            File.Delete(path);
            checkpoints.Clear();
            Debug.Log("Deleted Checkpoints at " + path);
        }
        else
        {
            Debug.LogError("Checkpoints file not found at " + path);
        }
    }
}


