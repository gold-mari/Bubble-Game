using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FlavorBomb : MonoBehaviour
{
    public BubbleSpawner spawner;

    public void Initialize(BubbleSpawner bubbleSpawner)
    {
        spawner = bubbleSpawner;
    }

    private void OnDestroy()
    {
        for (int i = 1; i < Bubble_Flavor_Methods.length; i++)
        {
            spawner.SpawnBubble(transform.position, (Bubble_Flavor)i, false);
        }
    }
}