using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class AccessibilitySettingsHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Header("Screenshake")]
    [SerializeField, Tooltip("A floatVar holding the 0-1 value that we scale our screenshake by.")]
    floatVar screenshakeScaling;
    [SerializeField, Tooltip("The slider controlling our screenshake scaling.")]
    Slider screenshakeSlider;
    [SerializeField, Tooltip("The slider controlling our screenshake scaling.")]
    SliderText screenshakeReadout;

    [Header("Controller Rumble")]
    [SerializeField, Tooltip("A floatVar holding the 0-1 value that we scale our rumble by.")]
    floatVar rumbleScaling;
    [SerializeField, Tooltip("The slider controlling our rumble scaling.")]
    Slider rumbleSlider;
    [SerializeField, Tooltip("The slider controlling our rumble scaling.")]
    SliderText rumbleReadout;

    [Header("Reduce Flashing")]
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
        if (PlayerPrefs.HasKey("RumbleAmountPref")) {
            rumbleScaling.value = PlayerPrefs.GetFloat("RumbleAmountPref");
        } else { // Default value.
            rumbleScaling.value = 1f;
        }
        if (PlayerPrefs.HasKey("ReduceFlashingPref")) {
            // If ReduceFlashingPref is 1, then reduce flashing is true.
            reduceFlashing.value = PlayerPrefs.GetInt("ReduceFlashingPref") == 1;
        } else { // Default value.
            reduceFlashing.value = false;
        }

        // Load UI and variable values from settings

        NaiveScreenshake[] screenshakes = FindObjectsOfType<NaiveScreenshake>(includeInactive:true);
        foreach (NaiveScreenshake s in screenshakes) {
            s.SetShakeScalingVar(screenshakeScaling);   
        }

        if (screenshakeSlider != null) screenshakeSlider.value = screenshakeScaling.value;
        if (screenshakeReadout != null) screenshakeReadout.UpdateText(screenshakeScaling.value);

        if (rumbleSlider != null) rumbleSlider.value = rumbleScaling.value;
        if (rumbleReadout != null) rumbleReadout.UpdateText(rumbleScaling.value);

        if (flashingToggle != null) flashingToggle.isOn = reduceFlashing.value;
    }

    private void OnDisable()
    {
        SaveToPrefs();
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetFloat("ScreenshakeAmountPref", screenshakeScaling.value);
        PlayerPrefs.SetFloat("RumbleAmountPref", rumbleScaling.value);
        PlayerPrefs.SetInt("ReduceFlashingPref", reduceFlashing.value ? 1 : 0);
    }

    // ==============================================================
    // Public manipulators
    // ==============================================================

    public void SetScreenshake(float value)
    {
        screenshakeScaling.value = value;
    }

    public void SetRumble(float value)
    {
        rumbleScaling.value = value;
    }

    public void SetReduceFlashing(bool value)
    {
        reduceFlashing.value = value;
    }
}
