using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorBombManager : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The Beat Reader present in the scene.")]
    private BeatReader beatReader;
    [SerializeField, Tooltip("The parent transform of all bubbles.")]
    private Transform bubbleParent;
    [SerializeField, Tooltip("The flavor bomb prefab.")]
    private GameObject flavorBomb;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The BubbleSpawner present in the scene. Obtained by searching from bubbleParent.
    private BubbleSpawner spawner;

    // ==============================================================
    // Initialization methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to find bubbleSpawner
        // via searching from bubbleParent.
        // ================

        spawner = bubbleParent.parent.GetComponent<BubbleSpawner>();
        beatReader.makeFlavorBomb += OnMakeFlavorBomb;
    }

    private void OnMakeFlavorBomb()
    {
        // Called when beatReader.makeFlavorBomb is invoked. Makes a flavor bomb!
        // ================
        
        // We randomly select a bubble to 'bomb-ify'. If we search all bubbles, and none are valid candidates, 
        // then we do nothing. In order to both randomly select an element AND know if we've picked everything, 
        // we maintain a list of bubbles that we select bubbles from until we bomb-ify one or we're all out.

        // Get a set of bubbles in our children. 
        List<Bubble> bubbles = new List<Bubble>(bubbleParent.GetComponentsInChildren<Bubble>());

        while(bubbles.Count > 0)
        {
            Bubble bubble = bubbles[Random.Range(0,bubbles.Count)];
            if (bubble.isBomb == true)
            {
                bubbles.Remove(bubble);
            }
            else
            {
                GameObject bomb = Instantiate(flavorBomb, bubble.transform);
                bomb.GetComponent<FlavorBomb>().Initialize(spawner);
                bubble.isBomb = true;
                return;
            }
        }

        if (bubbles.Count == 0)
        {
            print("No candidates could be found!");
        }            
    }
}
