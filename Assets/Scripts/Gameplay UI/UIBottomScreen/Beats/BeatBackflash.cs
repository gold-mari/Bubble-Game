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
        // ================

        if (animator)
        {
            if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.SingleSpawn)
            {
                spriteRenderer.flipY = false;
                animator.SetTrigger("MinorFlash");
            }
            else if ((currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.NONE
                   && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn) ||
                     (currentBeatmap.GetBeatType(tracker.nextLoopBeat) != BeatType.NONE
                   && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn))
            {
                spriteRenderer.flipY = true;
                animator.SetTrigger("MajorFlash");
            }
        }
    }
}
