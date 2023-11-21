using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCenter : MonoBehaviour
{
    [SerializeField, Tooltip("The BeatIndicator component above this object.")]
    private BeatIndicator beatIndicator;
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
    // The sprite renderer on this object.
    private SpriteRenderer spriteRenderer;

    private IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        currentBeatmap = beatIndicator.currentBeatmap;
        
        // Wait a frame before subscribing. Messy, but it's what we've gotta do.
        yield return null;
        tracker = beatIndicator.tracker;
        tracker.update += OnUpdate;
    }

    private void OnUpdate()
    {
        // Called every time the loop tracker updates. Triggers animations depending on what beat
        // the loop tracker is in.
        // ================

        if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.SingleSpawn)
        {
            spriteRenderer.sprite = SpriteFromType(BeatType.SingleSpawn);
            spriteRenderer.enabled = true;
        }
        else if (currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.NONE
              && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn)
        {
            spriteRenderer.sprite = SpriteFromType(currentBeatmap.GetBeatType(tracker.secondNextLoopBeat));
            spriteRenderer.enabled = true;
        }
        else if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.NONE
              && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) == BeatType.NONE)
        {
            spriteRenderer.enabled = false;
        }
    }

    private Sprite SpriteFromType(BeatType type)
    {
        // Returns the matching sprite based on a BeatType.
        // ================

        switch (type)
        {
            case BeatType.SingleSpawn:
            {
                return spawnSprite;
            }
            case BeatType.MassSpawn:
            {
                return massSpawnSprite;
            }
            case BeatType.FlipGravity:
            {
                return flipSprite;
            }
            case BeatType.MakeFlavorBomb:
            {
                return bombSprite;
            }
            case BeatType.HyperSpawn:
            {
                return HYPERsprite;
            }
            default:
            {
                return null;
            }
        }
    }
}
