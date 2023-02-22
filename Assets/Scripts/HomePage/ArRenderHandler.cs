using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArRenderHandler : MonoBehaviour
{
    private WebCamTexture _camTex;
    private Image _renderImage;

    void Start()
    {
        _camTex = new WebCamTexture();

        _renderImage = gameObject.GetComponent<Image>();
        _renderImage.material.mainTexture = _camTex;
        _camTex.Play();
    }
}
