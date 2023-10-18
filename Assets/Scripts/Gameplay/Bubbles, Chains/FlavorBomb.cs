using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public class FlavorBomb : MonoBehaviour
{
    // ==============================================================
    // Object references
    // ==============================================================

    // The bubble spawner present in the scene. Provided in Initialize().
    private BubbleSpawner spawner;
    private SpriteRenderer sprite;
    // The flavor of the bubble we're attached to
    private Bubble_Flavor ourFlavor = Bubble_Flavor.NONE;
    // A flavor bomb on a hyperbubble spawns other hyperbubbles.
    private bool spawnHyperbubbles = false;

    // ==============================================================
    // Animation parameters
    // ==============================================================

    // A color we use with Color.Lerp to make our rainbow Color translucent.
    private Color clearWhite = new Color(1,1,1,0);
    // Factor we multiply Time.time by to get our hue index. Default: 0.5f
    private float colorTimeFactor = 0.5f;
    // The min and max sizes, in world units, of this object for the pulsing animation. Default: 1.5f, 2f
    private float pulseMin = 1.5f, pulseMax = 2f;
    // Factor we multiply Time.time by to get our pulsation index. Default: 10f
    private float pulseTimeFactor = 10f;
    // The magnitude of our jitter animation, in world units. Default: 0.4f
    private float jitterMagnitude = 0.4f;
    // Factor we multiply Time.time by to get our rotation index. Default: 50f
    private float rotationTimeFactor = 50f;
    
    // ==============================================================
    // Initialization / Finalization methods
    // ==============================================================

    public void Initialize(BubbleSpawner bubbleSpawner, Bubble_Flavor flav, bool isHyperbubble)
    {
        // Initializes fields we reference elsewhere.
        // ================

        spawner = bubbleSpawner;
        ourFlavor = flav;
        spawnHyperbubbles = isHyperbubble;

        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        // Flavor bombs are destroyed and disabled when their chains are broken. Spawns bubbles.
        // ================

        // If this object is disabled because of scene termination, do no more.
        if(!gameObject.scene.isLoaded) 
        {
            return;
        }

        // DEBUG???
        // DEBUG!!!
        // i got a glock in my rari
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/FlavorBombExplode");
        // DEBUG???
        // DEBUG!!!
        // i got a glock in my rari

        for (int i = 1; i < Bubble_Flavor_Methods.length; i++)
        {
            // Don't spawn another bubble of the type that we were attached to.
            if ((Bubble_Flavor)i == ourFlavor)
            {
                continue;
            }

            Quaternion rot = Quaternion.Euler(0,0,360 * (i-1)/(Bubble_Flavor_Methods.length-1));
            spawner.SpawnBubble(transform.position, (Bubble_Flavor)i, rot * Vector2.right, spawnHyperbubbles);
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

        // Animate color (hue shift).
        sprite.color = Color.Lerp(Color.HSVToRGB((Time.time*colorTimeFactor)%1f,0.8f,1f), clearWhite, 0.4f);

        // Animate scale (pulsation).
        float pulseAmt = Mathf.Lerp(pulseMin, pulseMax, Mathf.Sin(pulseTimeFactor*Time.time));
        transform.localScale = new Vector3(pulseAmt,pulseAmt,1);

        // Animate position (jitter).
        transform.localPosition = new Vector3(Random.Range(-jitterMagnitude,jitterMagnitude), 
                                              Random.Range(-jitterMagnitude,jitterMagnitude), 
                                              0);

        // Animate rotation.
        transform.localRotation = Quaternion.Euler(0,0,rotationTimeFactor*Time.time%360f);
    }
}