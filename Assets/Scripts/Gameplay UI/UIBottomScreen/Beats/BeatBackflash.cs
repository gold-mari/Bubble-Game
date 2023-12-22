using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBackflash : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The BeatIndicator component above this object.")]
    private BeatIndicator beatIndicator;

    // ================================================================
    // Internal variables
    // ================================================================

    // The current beatmap!
    private Beatmap currentBeatmap;
    // A reference to our loop tracker.
    private LoopTracker tracker;
    // The sprite renderer on this object.
    private SpriteRenderer spriteRenderer;
    // The animator on this object.
    private Animator animator;

    // ================================================================
    // Initialization / finalization methods
    // ================================================================

    private IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentBeatmap = beatIndicator.currentBeatmap;
        
        // Wait a frame before subscribing. Messy, but it's what we've gotta do.
        yield return null;
        tracker = beatIndicator.tracker;
        tracker.update += OnUpdate;
    }

    private void OnDestroy()
    {
        tracker.update -= OnUpdate;
    }

    // ================================================================
    // Update methods
    // ================================================================

    void OnUpdate()
    {
        // Called every time the loop tracker updates. Triggers animations depending on what beat
        // the loop tracker is in.
        // Single spawns are 'warned' one beat in advance.
        // Anything bigger is 'warned' two AND one beats in advance.
        // ================

        if (animator)
        {
            // If the next beat is a single spawn, do a minor flash.
            if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.SingleSpawn)
            {
                spriteRenderer.flipY = false;
                print("minor flash");
                animator.SetTrigger("MinorFlash");
            }
            // If the second next beat OR the next beat is a higher-order beat (not single, not null), do a major flash.
            else if ((currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.NONE
                   && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn) ||
                     (currentBeatmap.GetBeatType(tracker.nextLoopBeat) != BeatType.NONE
                   && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn))
            {
                spriteRenderer.flipY = true;
                print("MAJOR FLASH");
                animator.SetTrigger("MajorFlash");
            }
        }
    }
}
