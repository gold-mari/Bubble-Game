using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BubbleColorHelper : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Tooltip("This object's base color. By default we get this from our Bubble_Flavor, but it can "
           + "updated at runtime if you wish.")]
    public Color baseColor;
    [Tooltip("A float (-1 to 1) which is used to lerp the base color towards white, if Brightness "
           + "is positive, or towards black, if Brightness is negative.")]
    public float brightness = 0;
    [Tooltip("A float (0 to 1) which changes this bubble's transparency.")]
    public float alpha = 1;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // Updated whenever brightness changes to store the previous value of brightness. We compare
    // this to the current value of brightness to detect if it has changed.
    private float lastBrightness;
    // The SpriteRenderer on this object.
    SpriteRenderer sprite;
    // The flavor associated with this color helper.
    BubbleFlavor flavor;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. Used to initialize all our
        // variables and define all our references.
        // ================

        brightness = lastBrightness = 0;
        
        flavor = transform.parent.GetComponent<Bubble>().bubbleFlavor;
        // Get the color from our parent's Bubble component.
        baseColor = BubbleFlavorMethods.GetColor(flavor);
        // Define sprite and apply baseColor to it.
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new(baseColor.r, baseColor.g, baseColor.b, alpha);
    }

    void Update()
    {
        // If brightness has changed, update brightness.
        if (brightness != lastBrightness) {
            UpdateBrightness();
            lastBrightness = brightness;
        }

        // If the base color has changed (because of settings, etc.), change the base color.
        if (baseColor != BubbleFlavorMethods.GetColor(flavor)) {
            baseColor = BubbleFlavorMethods.GetColor(flavor);
            sprite.color = new(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    void UpdateBrightness()
    {
        // Called when Brightness changes. If Brightness is 0, resets sprite color to
        // baseColor. If it is above 0, lerps towards white. If it is below 0, lerps
        // towards black.
        // ================

        Color baseAlpha = new(baseColor.r, baseColor.g, baseColor.b, alpha);

        if (brightness == 0) {
            sprite.color = baseAlpha;
        }
        else if (brightness > 0) {
            Color white = new(1,1,1,alpha);
            sprite.color = Color.Lerp(baseAlpha, white, brightness);
        }
        else { // if (Brightness < 0)
            Color black = new(0,0,0,alpha);
            sprite.color = Color.Lerp(baseAlpha, black, -brightness);
        }
    }

}
