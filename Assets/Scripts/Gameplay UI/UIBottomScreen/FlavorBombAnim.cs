using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlavorBombAnim : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("If true, listens to the Beat Reader present in the scene to "
                           + "hide and reveal this object. This is FALSE for functional flavor bombs, "
                           + "and is TRUE for the bomb indicator around the cursor.\n\nDefault: false")]
    private bool showOnBeat = false;
    [SerializeField, ShowIf("showOnBeat")]
    private BeatReader reader;
    [SerializeField, Tooltip("The boolVar holding whether or not we should reduce flashing.")]
    private boolVar reduceFlashing;

    // ==============================================================
    // Animation parameters and objects
    // ==============================================================

    // The sprite renderer on this object.
    private SpriteRenderer sprite;
    // Whether or not we should be hidden, as per showOnBeat.
    private bool showNow = false;
    // A color we use with Color.Lerp to make our rainbow Color translucent.
    private Color clearWhite = new Color(1,1,1,0);
    // Factor we multiply Time.time by to get our hue index. Default: 0.5f
    private float colorTimeFactor = 0.5f;
    [SerializeField, Tooltip("The min and max sizes, in world units, of this object for the pulsing "
                           + " animation.\n\nDefault: 1.5f, 2f")]
    private Vector2 pulseRange = new Vector2(1.5f, 2f);
    // Factor we multiply Time.time by to get our pulsation index. Default: 10f
    private float pulseTimeFactor = 10f;
    // The magnitude of our jitter animation, in world units. Default: 0.4f
    private float jitterMagnitude = 0.4f;
    // Factor we multiply Time.time by to get our rotation index. Default: 50f
    private float rotationTimeFactor = 50f;
    
    // ==============================================================
    // Initialization / Finalization methods
    // ==============================================================

    private void Start()
    {
        // Start is called before the first frame update.
        // ================

        sprite = GetComponent<SpriteRenderer>();

        if (showOnBeat)
        {
            reader.beforeMakeFlavorBomb += OnBeforeMakeFlavorBomb;
            reader.makeFlavorBomb += OnMakeFlavorBomb;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribes us from events.
        // ================

        if (showOnBeat)
        {
            reader.beforeMakeFlavorBomb -= OnBeforeMakeFlavorBomb;
            reader.makeFlavorBomb -= OnMakeFlavorBomb;
        }
    }

    // ==============================================================
    // Ongoing methods
    // ==============================================================

    private void Update()
    {
        // Update is called once per frame. Runs all our sprite / transform animations
        // on the Flavor Bomb object.
        // ================

        // If we always show OR we show now...
        if (!showOnBeat || showNow)
        {
            // Animate color (hue shift).
            sprite.color = Color.Lerp(Color.HSVToRGB((Time.time*colorTimeFactor)%1f,0.8f,1f), clearWhite, 0.4f);

            if (!reduceFlashing.value) {
                // Animate scale (pulsation).
                float pulseAmt = Mathf.Lerp(pulseRange.x, pulseRange.y, Mathf.Sin(pulseTimeFactor*Time.time));
                transform.localScale = new Vector3(pulseAmt,pulseAmt,1);

                // Animate position (jitter).
                transform.localPosition = new Vector3(
                    Random.Range(-jitterMagnitude,jitterMagnitude), 
                    Random.Range(-jitterMagnitude,jitterMagnitude), 
                    0
                );
            } else {
                transform.localScale = new Vector3(pulseRange.y, pulseRange.y, 1);
                transform.localPosition = Vector3.zero;
            }

            // Animate rotation.
            transform.localRotation = Quaternion.Euler(0,0,rotationTimeFactor*Time.time%360f);
        }
        // Otherwise, hide!
        else // if showOnBeat && !showNow
        {
            sprite.color = clearWhite;
        }
    }

    private void OnBeforeMakeFlavorBomb()
    {
        // Show the flavor bomb animation!
        // ================

        showNow = true;
    }

    private void OnMakeFlavorBomb()
    {
        // Hide the flavor bomb animation!
        // ================

        showNow = false;
    }
}