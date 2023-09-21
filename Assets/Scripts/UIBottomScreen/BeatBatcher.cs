using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBatcher : MonoBehaviour
{
    [Tooltip("The TimekeeperManager present in the scene.")]
    public TimekeeperManager timekeeper;
    [Tooltip("The BeatIconPopulator object.")]
    [SerializeField]
    private BeatIconPopulator populator;
    [Tooltip("The maximum number of beats to display at once.")]
    [SerializeField]
    private uint batchSize;
    // The size of the CURRENT batch. Useful in case batchSize doesn't evently fit into the loop,
    // and a partial batch is stuck to the end.
    private uint currentBatchSize;
    // The current beat in the loop.
    private uint currentLoopBeat = 1;
    // The current beat in the batch.
    private uint currentBatchBeat = 1;
    // The beats that (start, end) the current batch.
    private Vector2 batchStartEndBeats;
    // Whether or not we should track incoming beats and update counts.
    private bool shouldUpdate = false;

    void Start()
    {
        // Start is called before the first frame update. We use it to subscribe to
        // timekeeper events and to initialized batch stats.
        // ================

        timekeeper.beatUpdated += OnBeatUpdated;
        timekeeper.markerUpdated += OnMarkerUpdated;

        currentBatchSize = batchSize;
        batchStartEndBeats = new Vector2(1, currentBatchSize);
        populator.PlaceIcons((uint)batchStartEndBeats.x, (uint)batchStartEndBeats.y);
    }

    void OnDestroy()
    {
        // Use OnDestroy to unsubscribe from events.
        // ================

        timekeeper.beatUpdated -= OnBeatUpdated;
        timekeeper.markerUpdated -= OnMarkerUpdated;
    }

    void OnBeatUpdated()
    {
        // Called via the beatUpdated event from timekeeper. Increments our position in
        // the loop and the batch.
        // ================

        if (shouldUpdate) {
            // Update currentLoopBeat.

            // Case 1: We reach the end of the loop. Set everything back to 1.
            if (currentLoopBeat >= timekeeper.song.loopLength) {
                currentLoopBeat = 1;
                currentBatchBeat = 1;
                // Reset batch size.
                currentBatchSize = batchSize;
                batchStartEndBeats = new Vector2(1, currentBatchSize);
                // Update the icons via the populator.
                populator.ClearIcons();
                populator.PlaceIcons((uint)batchStartEndBeats.x, (uint)batchStartEndBeats.y);
            }

            // Case 2: We reach the end of a batch but not the loop. Increment our loop
            // counter but reset our batch counter.
            else if (currentBatchBeat >= currentBatchSize) {
                currentLoopBeat++;
                currentBatchBeat = 1;
                
                // If we have fewer beats in the loop left than the batch size, set our
                // current batch size to the number of beats left.
                uint beatsLeft = timekeeper.song.loopLength - currentLoopBeat + 1;
                if (beatsLeft < batchSize) {
                    currentBatchSize = beatsLeft;
                }

                // Get the start and end of the new batch.
                batchStartEndBeats = new Vector2(currentLoopBeat, currentLoopBeat+currentBatchSize-1);
                // Update the icons via the populator.
                populator.ClearIcons();
                populator.PlaceIcons((uint)batchStartEndBeats.x, (uint)batchStartEndBeats.y);
            }

            // Case 3: We have not reached the end of a batch or loop. Increment both.
            else {
                currentLoopBeat++;
                currentBatchBeat++;
            }

            print($"loop: {currentLoopBeat}\nbatch: {currentBatchBeat} - bounds: {batchStartEndBeats} - size: {currentBatchSize}");
        }
    }

    private void OnMarkerUpdated()
    {
        // Updates shouldUpdate based on the lastMarker.
        // ================

        if (timekeeper.timelineInfo.lastMarker == "dontSpawn") {
            shouldUpdate = false;
        }
        if (timekeeper.timelineInfo.lastMarker == "doSpawn") {
            // Update our beat counts to be the current beat in our first measure.
            // Go one behind, because markerUpdated is always called before beatUpdated.
            currentLoopBeat = (uint)timekeeper.timelineInfo.currentBeat - 1;
            currentBatchBeat = (uint)timekeeper.timelineInfo.currentBeat - 1;
            shouldUpdate = true;
        }
    }
}
