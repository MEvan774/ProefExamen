using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(ARCameraBackground))]
public class ArTexture : MonoBehaviour
{
    void Start()
    {
        var rawImage = GetComponent<RawImage>();
        var camBack = GetComponent<ARCameraBackground>().material;
        rawImage.material = camBack;
    }
}
