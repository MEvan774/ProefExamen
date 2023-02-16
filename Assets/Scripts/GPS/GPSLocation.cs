using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSLocation : MonoBehaviour
{
    [HideInInspector] public float latitude;
    [HideInInspector] public float longitude;


    void Start()
    {
        StartCoroutine(StartLocService());
    }
    
    private IEnumerator StartLocService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("User has not enabled GPS");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;

        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if(maxWait <= 0)
        {
            Debug.LogWarning("Timed out");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogWarning("Unable to determin device location");
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        yield break;
    }
}
