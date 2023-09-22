using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField, Tooltip("The main gameplay song to play in this scene.")]
    Song mainSong;
    [SerializeField, Tooltip("The 'Current Beatmap' variable in the scene.")]
    Beatmap currentBeatmap;
    
    // The instance of the currently-playing song.
    private FMOD.Studio.EventInstance instance;
    // The timeline handler tied to this music manager.
    private TimelineHandler handler;

    void Awake()
    {
        Debug.Assert(!mainSong.musicEvent.IsNull, "MusicManager Error: Start() failed. mainSong.musicEvent is null.");

        // Define the beatmap.
        currentBeatmap.Clear();
        uint loopLength = currentBeatmap.Populate(mainSong.beatmapFile);

        // Create the timeline handler.
        instance = FMODUnity.RuntimeManager.CreateInstance(mainSong.musicEvent);
        handler = new TimelineHandler(instance);
    }

    IEnumerator Start()
    {
        // Start the music.
        yield return new WaitForSeconds(2);
        instance.start();
        handler.StartDSPClock();
    }

    void Update()
    {
        handler.Update();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        handler.OnGUI();
    }
#endif

}
