using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCenter : MonoBehaviour
{
    [SerializeField, Tooltip("The BeatReader component present in the scene.")]
    private BeatReader beatReader;
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
    // The sprite renderer under this object.
    private SpriteRenderer[] spriteRenderers;
    // The animator on this object.
    private Animator animator;

    private IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentBeatmap = beatReader.GetBeatmap();
        
        // Wait a frame before subscribing. Messy, but it's what we've gotta do.
        yield return null;
        tracker = beatReader.GetLoopTracker();
        tracker.update += OnUpdate;
    }

    private void OnDestroy()
    {
        tracker.update -= OnUpdate;
    }

    private void OnUpdate()
    {
        // Called every time the loop tracker updates. Triggers animations depending on what beat
        // the loop tracker is in.
        // ================

        animator.ResetTrigger("Flash");
        animator.ResetTrigger("Fade");

        if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.SingleSpawn)
        {
            Sprite ourSprite = SpriteFromType(BeatType.SingleSpawn);
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                renderer.sprite = ourSprite;
            }
            animator.SetTrigger("Flash");
        }
        else if (currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.NONE
              && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn)
        {
            Sprite ourSprite = SpriteFromType(currentBeatmap.GetBeatType(tracker.secondNextLoopBeat));
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                renderer.sprite = ourSprite;
            }
            animator.SetTrigger("Flash");
        }
        else if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.NONE)
        {
            animator.SetTrigger("Fade");
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
