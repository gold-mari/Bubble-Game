using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bubble_Color {NONE, Red, Blue, Yellow, Green}

public class Bubble_Color_Methods : MonoBehaviour
{
    //===============================================================
    // Static fields
    //===============================================================

    // Returns the number of non-NONE Bubble_Colors
    public static int length = System.Enum.GetValues(typeof(Bubble_Color)).Length;

    //===============================================================
    // Static methods
    //===============================================================

    public static Bubble_Color random()
    {
        // Returns a random color that is not NONE.
        // ================

        return (Bubble_Color)Random.Range(1, length);
    }
    
    // Color references used with getSpriteColor.
    private static Color red = new Color(1f,0.2962264f,0.2962264f);
    private static Color blue = new Color(0.2980392f,0.6712846f,1f);
    private static Color yellow = new Color(0.9963511f,1f,0.2980392f);
    private static Color green = new Color(0.2980392f,1f,0.5218196f);

    public static Color getSpriteColor(Bubble_Color color)
    {
        // Depending on the inputted Bubble_Color, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        switch (color)
        {
            case Bubble_Color.Red:
                return red;
            case Bubble_Color.Blue:
                return blue;
            case Bubble_Color.Yellow:
                return yellow;
            case Bubble_Color.Green:
                return green;
        }
        // Default case.
        return Color.black;
    }
}
