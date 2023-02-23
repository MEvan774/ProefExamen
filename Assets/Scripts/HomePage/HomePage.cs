using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePage : MonoBehaviour
{
    [HideInInspector] public static float SoupAmount;

    [SerializeField] private Slider _soupProgressSlider;

    public void OnHomePageEnabled()
    {
        _soupProgressSlider.value = SoupAmount;
    }
}