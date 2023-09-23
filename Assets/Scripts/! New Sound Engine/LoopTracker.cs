using UnityEngine;

public class LoopTracker
{
    // ================================================================
    // Public properties
    // ================================================================

    // The current beat in the loop.
    public uint currentLoopBeat { get; private set; }
    // The number of beats in the batch. Optionally supplied in our constructor.
    public uint maxBatchSize { get; private set; }
    // The current batch size, in case we need to dip below our max to fit within our loop.
    public uint currentBatchSize { get; private set; }
    // The current beat in the batch.
    public uint currentBatchBeat { get; private set; }
    // The beats that start and end the current batch.
    public uint batchStartBeat { get; private set; }
    public uint batchEndBeat { get; private set; }
    // Actions for when we hit the start of a batch and a loop.
    public System.Action loopStart, batchStart;
    // An action for when we update our values, called at the end of OnBeatUpdate.
    // Ensures that any script that needs us always gets accurate loop counts, as opposed to
    // trying to sync with the TimelineHandler directly.
    public System.Action update;

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

        if (lastMarker == "dontSpawn") {
            shouldUpdate = false;
        }
        if (lastMarker == "doSpawn") {
            // Update our beat counts to be the current beat in our first measure.
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
            }
            shouldUpdate = true;
        }
    }
}