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
    // The value of our loopTracker's nextLoopBeat for the previous beat.
    // IMPORTANT: This is NOT necessarily the same as the beat type of the current beat-
    // this persists between beatmap changes.
    private BeatType lastNextType;

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
        tracker.switchMap += OnSwitchMap;
        tracker.update += OnTrackerUpdate;
    }

    private void OnDestroy()
    {
        tracker.update -= OnTrackerUpdate;
    }

    // ================================================================
    // Public accessors
    // ================================================================

    public Beatmap GetBeatmap()
    {
        return currentBeatmap;
    }

    public LoopTracker GetLoopTracker()
    {
        return tracker;
    }

    // ================================================================
    // Event-handling methods
    // ================================================================

    private void OnSwitchMap()
    {
        // Called when we switch maps. When switching on the same beat as a scheduled
        // beat event, the actual spawning of the even is overwritten by the mapchange.
        // By storing the lastNextType, we can invoke the event that went missing.
        // ================

        tracker.UpdateLoopLength(currentBeatmap.length);

        if (lastNextType == BeatType.SingleSpawn)
        {
            singleSpawn?.Invoke();
        }
        else if (lastNextType == BeatType.MassSpawn)
        {
            massSpawn?.Invoke();
        }
        else if (lastNextType == BeatType.FlipGravity)
        {
            flipGravity?.Invoke();
        }
        else if (lastNextType == BeatType.MakeFlavorBomb)
        {
            makeFlavorBomb?.Invoke();
        }
        else if (lastNextType == BeatType.HyperSpawn)
        {
            hyperSpawn?.Invoke();
        }
    }

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

        // Part 2: Parse the current beat.
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

        // Part 3: Get the last next beat.
        lastNextType = currentBeatmap.GetBeatType(tracker.nextLoopBeat);
    }
}