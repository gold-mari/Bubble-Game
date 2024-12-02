using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class AccessibilitySettingsHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

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

    // ==============================================================
    // Initializers/finalizers
    // ==============================================================

    public void Initialize()
    {
        // Load settings from prefs

        if (PlayerPrefs.HasKey("ScreenshakeAmountPref")) {
            screenshakeScaling.value = PlayerPrefs.GetFloat("ScreenshakeAmountPref");
        } else { // Default value.
            screenshakeScaling.value = 0.8f;
        }
        if (PlayerPrefs.HasKey("ReduceFlashingPref")) {
            // If ReduceFlashingPref is 1, then reduce flashing is true.
            reduceFlashing.value = PlayerPrefs.GetInt("ReduceFlashingPref") == 1;
        } else { // Default value.
            reduceFlashing.value = false;
        }

        // Load UI values from settings

        NaiveScreenshake[] screenshakes = FindObjectsOfType<NaiveScreenshake>(includeInactive:true);
        foreach (NaiveScreenshake s in screenshakes) {
            s.SetShakeScalingVar(screenshakeScaling);   
        }
        screenshakeSlider.value = screenshakeScaling.value;
        screenshakeReadout.UpdateText(screenshakeScaling.value);

        flashingToggle.isOn = reduceFlashing.value;
    }

    private void OnDisable()
    {
        SaveToPrefs();
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetFloat("ScreenshakeAmountPref", screenshakeScaling.value);
        PlayerPrefs.SetInt("ReduceFlashingPref", reduceFlashing.value ? 1 : 0);
    }

    // ==============================================================
    // Public manipulators
    // ==============================================================

    public void SetScreenshake(float value)
    {
        screenshakeScaling.value = value;
    }

    public void SetReduceFlashing(bool value)
    {
        reduceFlashing.value = value;
    }
}
