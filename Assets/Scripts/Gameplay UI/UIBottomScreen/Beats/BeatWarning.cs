using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatWarning : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The BeatIndicator component present in the scene.")]
    private BeatIndicator beatIndicator;

    // ================================================================
    // Internal variables
    // ================================================================

    // The current beatmap!
    private Beatmap currentBeatmap;
    // A reference to our loop tracker.
    private LoopTracker tracker;

    // ================================================================
    // Initialization / finalization methods
    // ================================================================

    protected virtual IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        currentBeatmap = beatIndicator.currentBeatmap;
        
        // Wait a frame before subscribing. Messy, but it's what we've gotta do.
        yield return null;
        tracker = beatIndicator.tracker;

        if (tracker != null)
        {
            tracker.update += OnUpdate;
        }
    }

    private void OnDestroy()
    {
        if (tracker != null)
        {
            tracker.update -= OnUpdate;
        }
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

        // If the next beat is a single spawn, do a minor flash.
        if (currentBeatmap.GetBeatType(tracker.nextLoopBeat) == BeatType.SingleSpawn)
        {
            OnMinorFlash();
            print("minor flash");
        }
        // If the second next beat OR the next beat is a higher-order beat (not single, not null), do a major flash.
        else if ((currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.NONE
                && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn) ||
                    (currentBeatmap.GetBeatType(tracker.nextLoopBeat) != BeatType.NONE
                && currentBeatmap.GetBeatType(tracker.secondNextLoopBeat) != BeatType.SingleSpawn))
        {
            OnMajorFlash();
        }
        // Otherwise, no flash.
        else 
        {
            OnNoFlash();
        }
    }

    protected virtual void OnMinorFlash()
    {
        return;
    }

    protected virtual void OnMajorFlash()
    {
        return;
    }

    protected virtual void OnNoFlash()
    {
        return;
    }
}
