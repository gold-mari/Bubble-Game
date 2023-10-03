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
    [SerializeField]
    private Color baseColor;
    [Tooltip("A float (-1 to 1) which is used to lerp the base color towards white, if Brightness "
           + "is positive, or towards black, if Brightness is negative.")]
    [SerializeField]
    private float brightness = 0;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // Updated whenever brightness changes to store the previous value of brightness. We compare
    // this to the current value of brightness to detect if it has changed.
    private float lastBrightness;
    // The SpriteRenderer on this object.
    SpriteRenderer sprite;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. Used to initialize all our
        // variables and define all our references.
        // ================

        brightness = lastBrightness = 0;
        
        // Get the color from our parent's Bubble component.
        baseColor = Bubble_Flavor_Methods.getColor(transform.parent.GetComponent<Bubble>().bubbleColor);
        // Define sprite and apply baseColor to it.
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = baseColor;
    }

    void Update()
    {
        // If brightness has changed, update brightness.
        if (brightness != lastBrightness) {
            UpdateBrightness();
            lastBrightness = brightness;
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

        if (brightness == 0) {
            sprite.color = baseColor;
        }
        else if (brightness > 0) {
            sprite.color = Color.Lerp(baseColor, Color.white, brightness);
        }
        else { // if (Brightness < 0)
            sprite.color = Color.Lerp(baseColor, Color.black, -brightness);
        }
    }

}
