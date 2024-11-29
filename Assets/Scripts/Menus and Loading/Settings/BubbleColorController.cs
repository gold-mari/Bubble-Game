using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleColorController : MonoBehaviour
{
    [SerializeField, Tooltip("The readout text we write the current flavor to.")]
    private TMP_Text readoutText;
    [SerializeField, Tooltip("The sliders for our 3 color components.")]
    private Slider hueSlider, saturationSlider, valueSlider;
    [SerializeField, Tooltip("The image display for the respective flavor.")]
    private Image sweetImage, saltyImage, sourImage, bitterImage, umamiImage;


    [SerializeField, ReadOnly]
    private Color[] workingColors = new Color[BubbleFlavorMethods.length-1];
    [SerializeField, ReadOnly]
    private int index = 0;


    private float H, S, V;


    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(70, 10, 50, 50), "Base!")) {
    //         PresetBase();
    //     }
    //     if (GUI.Button(new Rect(70, 70, 50, 50), "HiCon!")) {
    //         PresetHighContrast();
    //     }
    // }

    private void OnEnable()
    {
        LoadIntoWorking();
    }

    // private void OnDisable()
    // {
    //     SaveFromWorking();
    // }

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
            case 0: sweetImage.color = workingColors[i];    break;
            case 1: saltyImage.color = workingColors[i];    break;
            case 2: sourImage.color = workingColors[i];     break;
            case 3: bitterImage.color = workingColors[i];   break;
            case 4: umamiImage.color = workingColors[i];    break;
        }
    }

    private void InitializeHSV()
    {
        Color.RGBToHSV(workingColors[index], out float _H, out float _S, out float _V);

        H = hueSlider.value = _H;
        S = saturationSlider.value = _S;
        V = valueSlider.value = _V;
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