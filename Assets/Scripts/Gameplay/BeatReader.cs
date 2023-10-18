using System.Runtime.InteropServices;
using UnityEngine;

public class BeatReader : MonoBehaviour
{
    // ================================================================
    // Serialized and public fields
    // ================================================================

    [SerializeField, Tooltip("The music manager present in the scene.")]
    private MusicManager musicManager;
    [SerializeField, Tooltip("The beatmap we're reading.")]
    private Beatmap currentBeatmap;
    // Actions that are called depending on the current beat in our tracker, and the current Beatmap.
    // pre_Events are called one beat before the actual event.
    public System.Action beforeSingleSpawn, singleSpawn, 
                         beforeMassSpawn, massSpawn, 
                         beforeFlipGravity, flipGravity, 
                         beforeMakeFlavorBomb, makeFlavorBomb, 
                         beforeHyperSpawn, hyperSpawn;

    // ================================================================
    // Internal variables
    // ================================================================

    private LoopTracker tracker;

    // ================================================================
    // Initializers and finalizers
    // ================================================================

    private void Start()
    {
        tracker = new LoopTracker(musicManager.handler, currentBeatmap.length);
        tracker.update += OnTrackerUpdate;
    }

    private void OnDestroy()
    {
        tracker.update -= OnTrackerUpdate;
    }

    // ================================================================
    // Event-handling methods
    // ================================================================

    private void OnTrackerUpdate()
    {
        // Called every tracker update. Invokes actions depending on the value of the
        // beatmap at the tracker's currentLoopBeat.
        // ================

        // Part 1: Parse two beats from now.
        BeatType secondNextType = currentBeatmap.GetBeatType(tracker.secondNextLoopBeat);
        if (secondNextType == BeatType.SingleSpawn)
        {
            beforeSingleSpawn?.Invoke();
        }
        else if (secondNextType == BeatType.MassSpawn)
        {
            beforeMassSpawn?.Invoke();
        }
        else if (secondNextType == BeatType.FlipGravity)
        {
            beforeFlipGravity?.Invoke();
        }
        else if (secondNextType == BeatType.MakeFlavorBomb)
        {
            beforeMakeFlavorBomb?.Invoke();
        }
        else if (secondNextType == BeatType.HyperSpawn)
        {
            beforeHyperSpawn?.Invoke();
        }

        /*if (false) // code for one beat before
        {
            // Part 2: Parse the next beat.
            BeatType nextType = currentBeatmap.GetBeatType(tracker.nextLoopBeat);
            if (nextType == BeatType.SingleSpawn)
            {
                pre_singleSpawn?.Invoke();
            }
            else if (nextType == BeatType.MassSpawn)
            {
                pre_massSpawn?.Invoke();
            }
            else if (nextType == BeatType.FlipGravity)
            {
                pre_flipGravity?.Invoke();
            }
            else if (nextType == BeatType.MakeFlavorBomb)
            {
                pre_makeFlavorBomb?.Invoke();
            }
            else if (nextType == BeatType.HyperSpawn)
            {
                pre_hyperSpawn?.Invoke();
            }
        }*/

        // Part 3: Parse the current beat.
        BeatType type = currentBeatmap.GetBeatType(tracker.currentLoopBeat);
        if (type == BeatType.SingleSpawn)
        {
            singleSpawn?.Invoke();
        }
        else if (type == BeatType.MassSpawn)
        {
            massSpawn?.Invoke();
        }
        else if (type == BeatType.FlipGravity)
        {
            flipGravity?.Invoke();
        }
        else if (type == BeatType.MakeFlavorBomb)
        {
            makeFlavorBomb?.Invoke();
        }
        else if (type == BeatType.HyperSpawn)
        {
            hyperSpawn?.Invoke();
        }
    }
}