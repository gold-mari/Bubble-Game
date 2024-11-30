using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BubbleFlavor {NONE, Sweet, Salty, Sour, Bitter, Umami}

public class BubbleFlavorMethods
{
    //===============================================================
    // Utilities
    //===============================================================

    // Returns the number of Bubble_Flavors
    public static int length = System.Enum.GetValues(typeof(BubbleFlavor)).Length;

    public static Color HexToColor(string hex)
    {
        // Converts a 6-digit hex string to a color.
        // ================

        if (hex.Length != 6) {
            Debug.LogWarning($"BubbleFlavorMethods Error: HexToColor failed. Expecting 6 characters, recieved {hex.Length}.");
            return Color.white;
        }

        foreach (char c in hex) {
        	if (!"0123456789AaBbCcDdEeFf".Contains(c)) {
            	Debug.LogWarning($"BubbleFlavorMethods Error: HexToColor failed. String contains non-hex character: {hex}.");
                return Color.white;
            }
        }

        int r = System.Convert.ToInt32($"0x{hex[0]}{hex[1]}", 16);
        int g = System.Convert.ToInt32($"0x{hex[2]}{hex[3]}", 16);
        int b = System.Convert.ToInt32($"0x{hex[4]}{hex[5]}", 16);

        return new(r/255f, g/255f, b/255f);
    }

    public static string ColorToHex(Color color)
    {
        // Converts a color to a 6-digit hex string.
        // ================

        string r = ((int)(color.r*255)).ToString("X");
        string g = ((int)(color.g*255)).ToString("X");
        string b = ((int)(color.b*255)).ToString("X");

        return $"{r}{g}{b}";
    }

    //===============================================================
    // Private data
    //===============================================================

    // ================
    // Base colors
    // ================
    private static readonly Color colorSweet_b = new(0.781f,0.27f,0.9f);     // C745E6
    private static readonly Color colorSalty_b = new(0.298f,0.671f,1f);      // 4CABFF
    private static readonly Color colorSour_b = new(0.996f,1f,0.298f);       // FEFF4C
    private static readonly Color colorBitter_b = new(0.298f,1f,0.521f);     // 4CFF85
    private static readonly Color colorUmami_b = new(1f,0.296f,0.296f);      // FF4C4C
    

    // ================
    // Hi-contrast colors
    // ================
    private static readonly Color colorSweet_h = new(0.870f,0.329f,0.996f);  // DE54FE
    private static readonly Color colorSalty_h = new(0.207f,0.462f,0.709f);  // 3576B5
    private static readonly Color colorSour_h = new(0.980f,0.984f,0.733f);   // FAFBBB
    private static readonly Color colorBitter_h = new(0.176f,0.792f,0.368f); // 2DCA5E
    private static readonly Color colorUmami_h = new(0.603f,0.125f,0.125f);  // 9A2020

    // ================
    // Active colors
    // ================
    private static Color colorSweet = colorSweet_b;
    private static Color colorSalty = colorSalty_b;
    private static Color colorSour = colorSour_b;
    private static Color colorBitter = colorBitter_b;
    private static Color colorUmami = colorUmami_b;

    //===============================================================
    // Color accessors
    //===============================================================

    public static BubbleFlavor Random()
    {
        // Returns a random color that is not NONE.
        // ================

        return (BubbleFlavor)UnityEngine.Random.Range(1, length);
    }

    public static Color GetColor(BubbleFlavor flavor)
    {
        // Depending on the inputted Bubble_Flavor, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        return flavor switch
        {
            BubbleFlavor.Sweet => colorSweet,
            BubbleFlavor.Salty => colorSalty,
            BubbleFlavor.Sour => colorSour,
            BubbleFlavor.Bitter => colorBitter,
            BubbleFlavor.Umami => colorUmami,
            // Default case.
            _ => Color.black,
        };
    }

    public static Color[] GetColors()
    {
        return new[]{
            colorSweet,
            colorSalty, 
            colorSour, 
            colorBitter, 
            colorUmami
        };
    }
    
    public static Color[] GetBaseColors()
    {
        return new[]{
            colorSweet_b,
            colorSalty_b, 
            colorSour_b, 
            colorBitter_b, 
            colorUmami_b
        };
    }

    public static Color[] GetHiConColors()
    {
        return new[]{
            colorSweet_h,
            colorSalty_h, 
            colorSour_h, 
            colorBitter_h, 
            colorUmami_h
        };
    }

    //===============================================================
    // Color manipulators
    //===============================================================

    public static void SetColor(BubbleFlavor flavor, Color color)
    {
        // Depending on the inputted BubbleFlavor, sets the active
        // color for that flavor.
        // ================

        switch (flavor) {
            case BubbleFlavor.Sweet:    colorSweet = color;     break;
            case BubbleFlavor.Salty:    colorSalty = color;     break;
            case BubbleFlavor.Sour:     colorSour = color;      break;
            case BubbleFlavor.Bitter:   colorBitter = color;    break;
            case BubbleFlavor.Umami:    colorUmami = color;     break;
        }
    }

    public static void SetColors(Color[] colors)
    {
        // Sets the active colors for the bubbles.
        // Assumes the colors are in the canonical order:
        //  * Sweet
        //  * Salty
        //  * Sour
        //  * Bitter
        //  * Umami
        // ================

        colorSweet = colors[0];
        colorSalty = colors[1];
        colorSour = colors[2];
        colorBitter = colors[3];
        colorUmami = colors[4];
    }

    //===============================================================
    // Sprite accessors
    //===============================================================

    public static Sprite GetSprite(BubbleFlavor flavor, BubbleSpriteBinder binder)
    {
        // Depending on the inputted Bubble_Flavor, returns a system color that should be
        // applied to sprites. Serves as an accessor function to prevent repeating
        // "new Color(r,g,b)" all over the place.
        // ================

        return flavor switch
        {
            BubbleFlavor.Sweet => binder.sweetSprite,
            BubbleFlavor.Salty => binder.saltySprite,
            BubbleFlavor.Sour => binder.sourSprite,
            BubbleFlavor.Bitter => binder.bitterSprite,
            BubbleFlavor.Umami => binder.umamiSprite,
            // Default case.
            _ => binder.noneSprite,
        };
    }
}