using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlavorBomb : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================
    
    [SerializeField, Tooltip("The SFX event played when we explode.")]
    FMODUnity.EventReference explodeSFX;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The bubble spawner present in the scene. Provided in Initialize().
    private BubbleSpawner spawner;
    // The flavor of the bubble we're attached to
    private BubbleFlavor ourFlavor = BubbleFlavor.NONE;
    // A flavor bomb on a hyperbubble spawns other hyperbubbles.
    private bool spawnHyperbubbles = false;
    // The final position of the bubble we're attached to, cached when it begins getting destroyed.
    private Vector3 finalPosition = Vector3.negativeInfinity;

    // ==============================================================
    // Initialization / Finalization methods
    // ==============================================================

    public void Initialize(BubbleSpawner bubbleSpawner, BubbleFlavor flav, bool isHyperbubble)
    {
        // Initializes fields we reference elsewhere.
        // ================

        spawner = bubbleSpawner;
        ourFlavor = flav;
        spawnHyperbubbles = isHyperbubble;
    }

    public void StoreSpawnPosition(Vector3 position)
    {
        // Bubble objects are destroyed a few seconds after their chains are broken. Because of the
        // chain-breaking animation, bubbles move offscreen from where their 'true' final location was,
        // so we cache it manually.
        // This is called from a bubble's BubbleDestroy method.
        // ================

        finalPosition = position;
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

        Debug.Assert(finalPosition != Vector3.negativeInfinity, "FlavorBomb Error: OnDisable failed. "
                                                               +"StoreSpawnPosition was not called before OnDisable.");

        FMODUnity.RuntimeManager.PlayOneShot(explodeSFX);

        for (int i = 1; i < BubbleFlavorMethods.length; i++)
        {
            // Don't spawn another bubble of the type that we were attached to.
            if ((BubbleFlavor)i == ourFlavor)
            {
                continue;
            }

            Quaternion rot = Quaternion.Euler(0,0,360 * (i-1)/(BubbleFlavorMethods.length-1));
            spawner.SpawnBubble(finalPosition, (BubbleFlavor)i, rot * Vector2.right, spawnHyperbubbles);
        }
    }
}