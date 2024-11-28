using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilitySettingsHandler : MonoBehaviour
{
    [SerializeField, Tooltip("A floatVar holding the 0-1 value that we scale our screenshake by.")]
    floatVar screenshakeScaling;
    [SerializeField, Tooltip("The slider controlling our slider.")]
    Slider screenshakeSlider;
    [SerializeField, Tooltip("The slider controlling our slider.")]
    SliderText screenshakeReadout;


    public void Initialize()
    {
        NaiveScreenshake[] screenshakes = FindObjectsOfType<NaiveScreenshake>(includeInactive:true);
        foreach (NaiveScreenshake s in screenshakes) {
            s.SetShakeScalingVar(screenshakeScaling);   
        }

        screenshakeSlider.value = screenshakeScaling.value;
        screenshakeReadout.UpdateText(screenshakeScaling.value);
        print("Initialized acc settings handler");
        print($"value: {screenshakeScaling.value} --- ");
    }


    public void SetScreenshake(float value)
    {
        screenshakeScaling.value = value;
    }
}
