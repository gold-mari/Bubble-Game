using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bubble_Flavor {NONE, Sweet, Salty, Sour, Bitter, Umami}

public class Bubble_Flavor_Methods
{
    //===============================================================
    // Static fields
    //===============================================================

    // Returns the number of Bubble_Flavors
    public static int length = System.Enum.GetValues(typeof(Bubble_Flavor)).Length;

    //===============================================================
    // Static methods
    //===============================================================

    public static Bubble_Flavor random()
    {
        // Returns a random color that is not NONE.
        // ================

        return (Bubble_Flavor)Random.Range(1, length);
    }
    
    // Color references used with getSpriteColor.
    private static Color red = new Color(1f,0.2962264f,0.2962264f);     // FF4C4C
    private static Color blue = new Color(0.2980392f,0.6712846f,1f);    // 4CABFF
    private static Color yellow = new Color(0.9963511f,1f,0.2980392f);  // FEFF4C
    private static Color green = new Color(0.2980392f,1f,0.5218196f);   // 4CFF85
    private static Color purple = new Color(0.781f,0.27f,0.9f);         // C745E6

    public static Color getColor(Bubble_Flavor flavor)
    {
        // Depending on the inputted Bubble_Flavor, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        switch (flavor)
        {
            case Bubble_Flavor.Sweet:
                return purple;
            case Bubble_Flavor.Salty:
                return blue;
            case Bubble_Flavor.Sour:
                return yellow;
            case Bubble_Flavor.Bitter:
                return green;
            case Bubble_Flavor.Umami:
                return red;
        }
        // Default case.
        return Color.black;
    }

    public static Sprite getSprite(Bubble_Flavor flavor, BubbleSpriteBinder binder)
    {
        // Depending on the inputted Bubble_Flavor, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        switch (flavor)
        {
            case Bubble_Flavor.Sweet:
                return binder.sweetSprite;
            case Bubble_Flavor.Salty:
                return binder.saltySprite;
            case Bubble_Flavor.Sour:
                return binder.sourSprite;
            case Bubble_Flavor.Bitter:
                return binder.bitterSprite;
            case Bubble_Flavor.Umami:
                return binder.umamiSprite;
        }

        // Default case.
        return binder.noneSprite;
    }
}
