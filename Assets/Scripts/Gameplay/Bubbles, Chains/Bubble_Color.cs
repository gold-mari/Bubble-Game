using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bubble_Color {NONE, Red, Yellow, Green, Blue, Magenta}

public class Bubble_Color_Methods
{
    //===============================================================
    // Static fields
    //===============================================================

    // Returns the number of Bubble_Colors
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
    private static Color magenta = new Color(0.6764706f,0.2372549f,0.9f);

    public static Color getColor(Bubble_Color color)
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
            case Bubble_Color.Magenta:
                return magenta;
        }
        // Default case.
        return Color.black;
    }

    public static Sprite getSprite(Bubble_Color color, BubbleSpriteBinder binder)
    {
        // Depending on the inputted Bubble_Color, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        switch (color)
        {
            case Bubble_Color.Red:
                return binder.redSprite;
            case Bubble_Color.Yellow:
                return binder.yellowSprite;
            case Bubble_Color.Green:
                return binder.greenSprite;
            case Bubble_Color.Blue:
                return binder.blueSprite;
            case Bubble_Color.Magenta:
                return binder.magentaSprite;
        }
        // Default case.
        return binder.noneSprite;
    }
}
