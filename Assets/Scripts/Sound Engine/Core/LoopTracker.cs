using System.Runtime.InteropServices;
using UnityEngine;

public class LoopTracker
{
    // ================================================================
    // Public properties
    // ================================================================

    // The current beat in the loop.
    public uint currentLoopBeat { get; private set; }
    // The next beat in the loop.
    public uint nextLoopBeat 
    { 
        get
        {
            return (currentLoopBeat%loopSize)+1;
        } 
    }
    // The next NEXT beat in the loop.
    public uint secondNextLoopBeat 
    { 
        get
        {
            return ((currentLoopBeat+1)%loopSize)+1;
        } 
    }
    // The number of beats in the batch. Optionally supplied in our constructor.
    public uint maxBatchSize { get; private set; }
    // The current batch size, in case we need to dip below our max to fit within our loop.
    public uint currentBatchSize { get; private set; }
    // The current batch size, in case we need to know it ahead of time.
    public uint nextBatchSize { get; private set; }
    // The current beat in the batch.
    public uint currentBatchBeat { get; private set; }
    // The next beat in the batch.
    public uint nextBatchBeat 
    { 
        get
        {
            return (currentBatchBeat%currentBatchSize)+1;
        } 
    }
    // The beats that start and end the current batch.
    public uint batchStartBeat { get; private set; }
    public uint batchEndBeat { get; private set; }
    // The beats that start and end the NEXT batch.
    public uint nextBatchStart { get; private set; }
    public uint nextBatchEnd { get; private set; }
    // Actions for when we hit the start of a batch and a loop.
    public System.Action loopStart, batchStart;
    // An action for when we update our values, called at the end of OnBeatUpdate.
    // Ensures that any script that needs us always gets accurate loop counts, as opposed to
    // trying to sync with the TimelineHandler directly.
    public System.Action update;
    // An action called when we encounter the "dontTrack" and "doTrack" labels.
    // Used to force refresh the BeatIconPopulator.
    public System.Action dontTrack, doTrack;
    // An action called when we switch maps. Primarily used to have the beat reader call any
    // actions that were shouted over by a map switch.
    public System.Action switchMap;

    // ================================================================
    // Internal variables
    // ================================================================

    // The timeline handler on our music manager. Supplied in our constuctor.
    private TimelineHandler handler;
    // The number of beats in the loop. Supplied in our constructor.
    private uint loopSize = 0;
    // Whether or not we should track incoming beats and update counts.
    private bool shouldUpdate = false;

    // ================================================================
    // Initializers and finalizers
    // ================================================================

    public LoopTracker(TimelineHandler timelineHandler, uint loop, uint batch)
    {
        // Constructor with the optional batch parameter.
        // ================    

        handler = timelineHandler;
        maxBatchSize = loopSize = loop;
        // If batch size is bigger than loop size, just use loop size.
        if (batch <= loop)
        {
            maxBatchSize = batch;
        }

        Initialize(timelineHandler);
    }

    public LoopTracker(TimelineHandler timelineHandler, uint loop)
    {
        // Constructor without the optional batch parameter.
        // batchSize defaults to loopSize.
        // ================

        handler = timelineHandler;
        maxBatchSize = loopSize = loop;

        Initialize(timelineHandler);
    }

    private void Initialize(TimelineHandler timelineHandler)
    {
        // Common intialization functions for both constructors.
        // ================

        handler.beatUpdated += OnBeatUpdated;
        handler.markerUpdated += OnMarkerUpdated;

        currentBatchSize = maxBatchSize;
        batchStartBeat = 1;
        batchEndBeat = currentBatchSize;

        // So that the batchStart and loopStart actions are invoked at the start.
        currentLoopBeat = loopSize;
    }

    ~LoopTracker()
    {
        // Finalizer.
        // Used to unsubscribe from events.
        // ================

        handler.beatUpdated -= OnBeatUpdated;
        handler.markerUpdated -= OnMarkerUpdated;
    }

    // ================================================================
    // Helper methods
    // ================================================================

    private uint BeatsInNextBatch()
    {
        // Returns the number of beats in the next batch.
        // ================

        uint beatsLeft = loopSize - currentLoopBeat + 1;
        uint nextSize = beatsLeft - currentBatchSize;
        // If the calculated next size is 0, then we're at the last batch of the loop.
        // Alternately, if there's MORE than the max size left, we've still got a ways to go.
        // The next batch should be maximum size!
        if (nextSize == 0 || nextSize > maxBatchSize)
        {
            return maxBatchSize;
        }
        // Otherwise, a partial batch is up next. Return that.
        return nextSize;
    }

    private void SetNextBatchStartEnd()
    {
        // Sets the next start/end beats properties.
        // ================

        // If we are at the last batch, we should start from 1 next time.
        if (batchEndBeat == loopSize)
        {
            nextBatchStart = 1;
        }
        else
        {
            nextBatchStart = batchEndBeat + 1;
        }

        nextBatchEnd = nextBatchStart + nextBatchSize - 1;
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
                currentBatchBeat = 1;
                // Reset batch size and bounds.
                currentBatchSize = maxBatchSize;
                batchStartBeat = 1;
                batchEndBeat = currentBatchSize;
                // Calculate the size of the next batch.
                nextBatchSize = BeatsInNextBatch();
                SetNextBatchStartEnd();
                // Invoke actions!
                batchStart?.Invoke();
                loopStart?.Invoke();
            }

            // Case 2: We reach the end of a batch but not the loop. Increment our loop
            // counter but reset our batch counter.
            else if (currentBatchBeat >= currentBatchSize) {
                currentLoopBeat++;
                currentBatchBeat = 1;
                
                // If we have fewer beats in the loop left than the max batch size, set our
                // current batch size to the number of beats left.
                uint beatsLeft = loopSize - currentLoopBeat + 1;
                if (beatsLeft < maxBatchSize) {
                    currentBatchSize = beatsLeft;
                }

                // Get the start and end of the new batch.
                batchStartBeat = currentLoopBeat;
                batchEndBeat = currentLoopBeat+currentBatchSize-1;

                // Calculate the size of the next batch.
                nextBatchSize = BeatsInNextBatch();
                SetNextBatchStartEnd();

                // Invoke actions!
                batchStart?.Invoke();
            }

            // Case 3: We have not reached the end of a batch or loop. Increment both.
            else {
                currentLoopBeat++;
                currentBatchBeat++;
            }

            update?.Invoke();
        }
    }

    private void OnMarkerUpdated(string lastMarker)
    {
        // Updates shouldUpdate based on the lastMarker.
        // ================

        if (lastMarker == "dontTrack") {
            shouldUpdate = false;
            dontTrack?.Invoke();
        }
        if (lastMarker == "doTrack") {
            // If this is the not the first beat, go one behind, because markerUpdated is
            // always called before beatUpdated.
            if (handler.timelineInfo.currentBeat != 1)
            {
                currentBatchBeat = currentLoopBeat = (uint)handler.timelineInfo.currentBeat - 1;   
            }
            else
            {
                // Otherwise, set our current beat to be the last possible loop beat. This 
                // forces Case 1 in OnBeatUpdated, which in turn fires off our batchStart
                // and loopStart events as is proper.
                currentLoopBeat = loopSize;
                currentBatchBeat = currentBatchSize;
                // Set us in the final batch.
                batchEndBeat = currentLoopBeat;
                batchStartBeat = batchEndBeat-currentBatchSize+1;
            }
            shouldUpdate = true;
            // Find the stats of the next batch! Used in some functions subscribed to doTrack.
            nextBatchSize = BeatsInNextBatch();
            SetNextBatchStartEnd();
            doTrack?.Invoke();
        }
        else
        {
            if (lastMarker.Contains("switchMap"))
            {
                switchMap?.Invoke();
            }
        }
    }
}