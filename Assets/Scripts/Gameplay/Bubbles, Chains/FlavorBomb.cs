using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlavorBomb : MonoBehaviour
{
    // ==============================================================
    // Object references
    // ==============================================================

    // The bubble spawner present in the scene. Provided in Initialize().
    private BubbleSpawner spawner;
    // The flavor of the bubble we're attached to
    private Bubble_Flavor ourFlavor = Bubble_Flavor.NONE;
    // A flavor bomb on a hyperbubble spawns other hyperbubbles.
    private bool spawnHyperbubbles = false;

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
}