using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIconPopulator : MonoBehaviour
{
    [SerializeField, Tooltip("The BeatIndicator component above this object.")]
    private BeatIndicator beatIndicator;
    [SerializeField, Tooltip("Where the BeatIndicator 'Offset' object is centered, in worldspace units.")]
    private Vector2 center;
    [SerializeField]
    private Vector2 angleRange;
    [SerializeField]
    private GameObject icon;
    [SerializeField]
    private Color iconColor;
    [SerializeField]
    private Sprite spawnSprite;
    [SerializeField]
    private Sprite massSpawnSprite;
    [SerializeField]
    private Sprite flipSprite;
    [SerializeField]
    private Sprite bombSprite;
    [SerializeField]
    private Sprite HYPERsprite;
    
    // The current beatmap!
    private Beatmap currentBeatmap;
    // A reference to our loop tracker.
    private LoopTracker tracker;

    private IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        currentBeatmap = beatIndicator.currentBeatmap;
        
        // Wait a frame before subscribing. Messy, but it's what we've gotta do.
        yield return null;
        tracker = beatIndicator.tracker;
        tracker.update += OnUpdate;
        tracker.doTrack += OnDoTrack;
        tracker.dontTrack += OnDontTrack;

        PlaceIcons(tracker.batchStartBeat, tracker.batchEndBeat);
    }

    private void OnDestroy()
    {
        tracker.update -= OnUpdate;
        tracker.doTrack -= OnDoTrack;
        tracker.dontTrack -= OnDontTrack;
    }

    private void OnUpdate()
    {
        // Called every time we get an update from our tracker. If we're in the final beat,
        // clears icons and places the next set.
        // ================
        
        //print($"{tracker.currentBatchBeat}, {tracker.currentBatchSize}");
        if (tracker.currentBatchBeat == tracker.currentBatchSize)
        {
            ClearIcons();
            PlaceIcons(tracker.nextBatchStart, tracker.nextBatchEnd);
        }
    }

    private void OnDoTrack()
    {
        // Called when our tracker fires off the doTrack action. Force refreshes our icons.
        // ================

        ClearIcons();
        PlaceIcons(tracker.nextBatchStart, tracker.nextBatchEnd);
    }

    private void OnDontTrack()
    {
        // Called when our tracker fires off the dontTrack action. Clears our icons.
        // ================

        ClearIcons();
    }

    public void PlaceIcons(uint min, uint max)
    {
        float lerpAmount = 0;
        float spawnAngle = 0;
        uint batchLength = max-min+1;

        for (uint i = min; i <= max; i++)
        {
            uint batch_i = i-min+1;
            lerpAmount = (float)batch_i/(batchLength+1);
            spawnAngle = Mathf.Lerp(angleRange.x, angleRange.y, lerpAmount);

            BeatType type = currentBeatmap.GetBeatType(i);
            // If the type of this beat is not NONE,
            if (type != BeatType.NONE) {
                // Spawn an object.
                GameObject iconObj = Instantiate(icon, (Vector3)center, Quaternion.Euler(0,0,spawnAngle), transform);
                // Initialize its beatIcon.
                iconObj.GetComponentInChildren<BeatIcon>().Initialize(i, tracker, type == BeatType.SingleSpawn);
                // Set its color.
                iconObj.GetComponentInChildren<SpriteRenderer>().color = iconColor;

                // Set its sprite according to its type. Default to the spawnSprite.
                Sprite s = spawnSprite;
                switch (type)
                {
                    case BeatType.MassSpawn:
                    {
                        s = massSpawnSprite;
                        break;
                    }
                    case BeatType.FlipGravity:
                    {
                        s = flipSprite;
                        break;
                    }
                    case BeatType.MakeFlavorBomb:
                    {
                        s = bombSprite;
                        break;
                    }
                    case BeatType.HyperSpawn:
                    {
                        s = HYPERsprite;
                        break;
                    }
                }

                foreach (SpriteRenderer renderer in iconObj.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.sprite = s;
                }
            }
        }
    }

    public void ClearIcons()
    {
        // Destroys all extant icons by destroying all children.
        // ================

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
