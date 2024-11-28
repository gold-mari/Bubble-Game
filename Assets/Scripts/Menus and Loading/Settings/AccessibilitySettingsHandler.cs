using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilitySettingsHandler : MonoBehaviour
{
    [SerializeField, Tooltip("A floatVar holding the 0-1 value that we scale our screenshake by.")]
    floatVar screenshakeScaling;
    [SerializeField, Tooltip("The slider controlling our screenshake scaling.")]
    Slider screenshakeSlider;
    [SerializeField, Tooltip("The slider controlling our screenshake scaling.")]
    SliderText screenshakeReadout;
    [SerializeField, Tooltip("A boolVar holding the bool value for reducing flashing.")]
    boolVar reduceFlashing;
    [SerializeField, Tooltip("The toggle controlling our flashign reduction.")]
    Toggle flashingToggle;


    public void Initialize()
    {
        NaiveScreenshake[] screenshakes = FindObjectsOfType<NaiveScreenshake>(includeInactive:true);
        foreach (NaiveScreenshake s in screenshakes) {
            s.SetShakeScalingVar(screenshakeScaling);   
        }

        screenshakeSlider.value = screenshakeScaling.value;
        screenshakeReadout.UpdateText(screenshakeScaling.value);

        flashingToggle.isOn = reduceFlashing.value;
    }

    public void SetScreenshake(float value)
    {
        screenshakeScaling.value = value;
    }

    public void SetReduceFlashing(bool value)
    {
        reduceFlashing.value = value;
    }
}
