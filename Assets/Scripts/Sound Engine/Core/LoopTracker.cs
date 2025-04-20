using UnityEngine;

public class LoopTracker
{
    // ================================================================
    // Public properties
    // ================================================================

    // The current beat in the loop.
    public uint currentLoopBeat { get; private set; }
    // The number of beats in the loop. Supplied in our constructor.
    public uint loopSize => beatmap.length;
    // The next beat in the loop.
    public uint nextLoopBeat 
    { 
        get { return (currentLoopBeat%loopSize)+1; } 
    }
    // The next NEXT beat in the loop.
    public uint secondNextLoopBeat 
    { 
        get { return ((currentLoopBeat+1)%loopSize)+1; } 
    }

    // An action for when we update our values, called at the end of OnBeatUpdate.
    // Ensures that any script that needs us always gets accurate loop counts, as opposed to
    // trying to sync with the TimelineHandler directly.
    public System.Action update;
    // An action called when we switch maps. Primarily used to have the beat reader call any
    // actions that were shouted over by a map switch.
    public System.Action switchMap;

    // ================================================================
    // Internal variables
    // ================================================================

    // The music manager we're tracking.
    private MusicManager manager;
    // The Beatmap variable tracking the current beatmap.
    private Beatmap beatmap;
    // The timeline handler on our music manager. Supplied in our constuctor.
    private TimelineHandler handler => manager.handler;
    // Whether or not we should track incoming beats and update counts.
    private bool shouldUpdate = false;

    // ================================================================
    // Initializers and finalizers
    // ================================================================

    public LoopTracker(MusicManager manager, Beatmap beatmap)
    {
        // Sets our references.
        // ================
        
        this.beatmap = beatmap;
        this.manager = manager;

        if (handler != null) {
            handler.beatUpdated += OnBeatUpdated;
            handler.markerUpdated += OnMarkerUpdated;
        }

        // So that the batchStart and loopStart actions are invoked at the start.
        currentLoopBeat = loopSize;
    }

    ~LoopTracker()
    {
        // Finalizer.
        // Used to unsubscribe from events.
        // ================

        if (handler != null) {
            handler.beatUpdated -= OnBeatUpdated;
            handler.markerUpdated -= OnMarkerUpdated;
        }
    }

    // ================================================================
    // Event-handling methods
    // ================================================================

    void OnBeatUpdated()
    {
        // Called via the beatUpdated event from timekeeper. Increments our position in
        // the loop and the batch.
        // ================

        if (shouldUpdate) {
            // Case 1: We reach the end of the loop. Set everything back to 1.
            if (currentLoopBeat >= loopSize) {
                currentLoopBeat = 1;
            }
            // Case 2: We have not reached the end of a batch or loop. Increment both.
            else {
                currentLoopBeat++;
            }

            update?.Invoke();
        }

        // Debug.Log(currentLoopBeat);
    }

    private void OnMarkerUpdated(string lastMarker)
    {
        // Updates shouldUpdate based on the lastMarker.
        // ================

        if (lastMarker.Contains("switchMap")) {
            switchMap?.Invoke();
            ResetLoop();
        }

        if (lastMarker.Contains("endlessNextSong")) {
            shouldUpdate = false;
            ResetLoop(true);
        }

        if (lastMarker == "dontTrack") {
            shouldUpdate = false;
        }

        if (lastMarker == "doTrack") {
            ResetLoop();
        }
    }

    private void ResetLoop(bool forceToEnd=false)
    {
        // Resets the loop to be just before the first position.
        // ================

        // If this is the not the first beat, go one behind, because markerUpdated is
        // always called before beatUpdated.
        if (!forceToEnd && handler.timelineInfo.currentBeat > 1) {
            currentLoopBeat = (uint)handler.timelineInfo.currentBeat - 1;
        } else {
            // Otherwise, set our current beat to be the last possible loop beat. This 
            // forces Case 1 in OnBeatUpdated, which in turn fires off our batchStart
            // and loopStart events as is proper.
            currentLoopBeat = loopSize;
        }

        Debug.Log($"Reset currentLoopBeat to {currentLoopBeat}");
        shouldUpdate = true;
    }
}