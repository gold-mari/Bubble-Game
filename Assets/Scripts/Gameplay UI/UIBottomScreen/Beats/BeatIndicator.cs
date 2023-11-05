using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
{
    [Tooltip("The music manager present in the scene.")]
    public MusicManager musicManager;
    [Tooltip("The current beatmap. Used to get loop length.")]
    public Beatmap currentBeatmap;
    [SerializeField, Tooltip("How many beats are displayed at once.\n\nDefault: 8")]
    private uint batchSize = 8;

    // A reference to our loop tracker.
    public LoopTracker tracker;
    
    private void Start()
    {
        // Start is called before the first frame update. We use it to create the tracker.
        // ================

        // Initialize the tracker.
        tracker = new LoopTracker(musicManager.handler, currentBeatmap.length, batchSize, true);
    }
}