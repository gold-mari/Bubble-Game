using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class BubbleColorController : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The readout text we write the current flavor to.")]
    private TMP_Text readoutText;
    [SerializeField, Tooltip("The sliders for our 3 color components.")]
    private Slider hueSlider, saturationSlider, valueSlider;
    [SerializeField, Tooltip("The image display for the respective flavor.")]
    private Image sweetImage, saltyImage, sourImage, bitterImage, umamiImage;

    // ==============================================================
    // Inspector monitors
    // ==============================================================

    [SerializeField, ReadOnly]
    private Color[] workingColors = new Color[BubbleFlavorMethods.length-1];
    [SerializeField, ReadOnly]
    private int index = 0;

    // ==============================================================
    // Misc. internal variables
    // ==============================================================

    private float H, S, V;

    // ==============================================================
    // Initializers/finalizers
    // ==============================================================

    public void Initialize()
    {
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, on Awake().
        // ================

        // Load settings from prefs into BubbleFlavorMethods

        Color[] prefColors = new Color[5];
        Color[] baseColors = BubbleFlavorMethods.GetBaseColors();

        string[] prefKeys = new string[5] {
            "SweetColorPref",
            "SaltyColorPref",
            "SourColorPref",
            "BitterColorPref",
            "UmamiColorPref"
        };

        // Loop through each of the PlayerPref keys, and load their values.
        for (int i = 0; i < prefKeys.Length; i++) {
            string key = prefKeys[i];

            if (PlayerPrefs.HasKey(key)) {
                string hex = PlayerPrefs.GetString(key);
                prefColors[i] = BubbleFlavorMethods.HexToColor(hex);
            } else { // Default value.
                prefColors[i] = baseColors[i];
            }
        }

        BubbleFlavorMethods.SetColors(prefColors);
    }

    private void OnEnable()
    {
        // Loads from BubbleFlavorMethods to our local working copies.
        // Also updates UI.
        LoadIntoWorking();
        // Reset our array index.
        index = 0;
    }

    private void OnDisable()
    {
        SaveFromWorking();
        SaveToPrefs();
    }

    private void SaveToPrefs()
    {
        // Saves the active colors from BubbleFlavorMethods to PlayerPrefs.
        // IMPORTANT: SaveFromWorking() SHOULD BE CALLED BEFORE THIS!
        // ================

        Color[] colors = BubbleFlavorMethods.GetColors();

        PlayerPrefs.SetString("SweetColorPref", BubbleFlavorMethods.ColorToHex(colors[0]));
        PlayerPrefs.SetString("SaltyColorPref", BubbleFlavorMethods.ColorToHex(colors[1]));
        PlayerPrefs.SetString("SourColorPref", BubbleFlavorMethods.ColorToHex(colors[2]));
        PlayerPrefs.SetString("BitterColorPref", BubbleFlavorMethods.ColorToHex(colors[3]));
        PlayerPrefs.SetString("UmamiColorPref", BubbleFlavorMethods.ColorToHex(colors[4]));
    }

    //===============================================================
    // Save/Load working colors
    //===============================================================

    public void LoadIntoWorking()
    {
        Array.Copy(BubbleFlavorMethods.GetColors(), workingColors, BubbleFlavorMethods.length-1);

        for (int i = 0; i < workingColors.Length; i++) {
            UpdatePreview(i);
        }

        InitializeHSV();
    }

    public void PresetBase()
    {
        Array.Copy(BubbleFlavorMethods.GetBaseColors(), workingColors, BubbleFlavorMethods.length-1);

        for (int i = 0; i < workingColors.Length; i++) {
            UpdatePreview(i);
        }

        InitializeHSV();
    }

    public void PresetHighContrast()
    {
        Array.Copy(BubbleFlavorMethods.GetHiConColors(), workingColors, BubbleFlavorMethods.length-1);

        for (int i = 0; i < workingColors.Length; i++) {
            UpdatePreview(i);
        }

        InitializeHSV();
    }

    public void SaveFromWorking()
    {
        BubbleFlavorMethods.SetColors(workingColors);
    }

    //===============================================================
    // HSV component manipulators
    //===============================================================

    public void SetHue(float amount)
    {
        H = amount;
        workingColors[index] = Color.HSVToRGB(H, S, V);

        UpdatePreview(index);
    }

    public void SetSaturation(float amount)
    {
        S = amount;
        workingColors[index] = Color.HSVToRGB(H, S, V);
        
        UpdatePreview(index);
    }
    
    public void SetValue(float amount)
    {
        V = amount;
        workingColors[index] = Color.HSVToRGB(H, S, V);
        
        UpdatePreview(index);
    }

    //===============================================================
    // Display previews
    //===============================================================

    private void UpdatePreview(int i)
    {
        switch (i) {
            case 0: if (sweetImage != null) sweetImage.color = workingColors[i];    break;
            case 1: if (saltyImage != null) saltyImage.color = workingColors[i];    break;
            case 2: if (sourImage != null) sourImage.color = workingColors[i];      break;
            case 3: if (bitterImage != null) bitterImage.color = workingColors[i];  break;
            case 4: if (umamiImage != null) umamiImage.color = workingColors[i];    break;
        }
    }

    private void InitializeHSV()
    {
        Color.RGBToHSV(workingColors[index], out float H, out float S, out float V);

        if (hueSlider != null) hueSlider.value = H;
        if (saturationSlider != null) saturationSlider.value = S;
        if (valueSlider != null) valueSlider.value = V;
    }

    //===============================================================
    // Index manipulators
    //===============================================================

    public void Increment() 
    { 
        index = (index+1)%workingColors.Length;
        readoutText.text = ((BubbleFlavor)(index+1)).ToString();

        InitializeHSV();
    }

    public void Decrement() 
    {
        index = (index == 0) ? workingColors.Length-1 : index-1;
        readoutText.text = ((BubbleFlavor)(index+1)).ToString();

        InitializeHSV();
    }
}