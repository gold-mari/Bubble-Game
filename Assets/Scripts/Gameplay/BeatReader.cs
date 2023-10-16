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
    public System.Action singleSpawn, 
                         pre_massSpawn, massSpawn, 
                         pre_flipGravity, flipGravity, 
                         pre_makeFlavorBomb, makeFlavorBomb, 
                         pre_hyperSpawn, hyperSpawn;

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