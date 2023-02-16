using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSLocation : MonoBehaviour
{

    float latitude;
    float longitude;

    void Start()
    {
        StartCoroutine(StartLocService());
    }

    IEnumerator StartLocService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;

        //----

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if(maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device loc");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        yield break;
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            //yield break;
        }
        else
        {
            //acces granted
        }
    }
}
