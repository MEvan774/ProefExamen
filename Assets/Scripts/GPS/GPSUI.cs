using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSUI : MonoBehaviour
{
    GPSLocation gpsLoc;

    public Text[] texts;


    // Start is called before the first frame update
    void Start()
    {
        gpsLoc = GetComponent<GPSLocation>();
    }

    // Update is called once per frame
    void Update()
    {
        texts[0].text = "Latitude = " + gpsLoc.latitude.ToString();
        texts[1].text = "Longitude = " + gpsLoc.longitude.ToString();
    }
}
