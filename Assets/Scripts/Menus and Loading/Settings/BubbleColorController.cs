using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BubbleColorController : MonoBehaviour
{
    [SerializeField, Tooltip("The image display for the respective flavor.")]
    private Image sweetImage, saltyImage, sourImage, bitterImage, umamiImage;

    private float H, S, V;

    [SerializeField, ReadOnly]
    private Color[] workingColors = new Color[BubbleFlavorMethods.length-1];
    [SerializeField, ReadOnly]
    private int index = 0;

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

    private void OnDisable()
    {
        SaveFromWorking();
    }

    //===============================================================
    // Save/Load working colors
    //===============================================================

    public void LoadIntoWorking()
    {
        Array.Copy(BubbleFlavorMethods.GetColors(), workingColors, BubbleFlavorMethods.length-1);
    }

    public void PresetBase()
    {
        Array.Copy(BubbleFlavorMethods.GetBaseColors(), workingColors, BubbleFlavorMethods.length-1);

        for (int i = 0; i < workingColors.Length; i++) {
            UpdatePreview(i);
        }
    }

    public void PresetHighContrast()
    {
        Array.Copy(BubbleFlavorMethods.GetHiConColors(), workingColors, BubbleFlavorMethods.length-1);

        for (int i = 0; i < workingColors.Length; i++) {
            UpdatePreview(i);
        }
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

    private void UpdatePreview(int index)
    {
        switch (index) {
            case 0: sweetImage.color = workingColors[index];    break;
            case 1: saltyImage.color = workingColors[index];    break;
            case 2: sourImage.color = workingColors[index];     break;
            case 3: bitterImage.color = workingColors[index];   break;
            case 4: umamiImage.color = workingColors[index];    break;
        }
    }

    //===============================================================
    // Index manipulators
    //===============================================================

    public void Increment() 
    { 
        index = (index+1)%workingColors.Length;
    }

    public void Decrement() 
    {
        index = (index == 0) ? workingColors.Length-1 : index-1;
    }
}