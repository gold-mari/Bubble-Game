using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MusicManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The main gameplay song to play in this scene.")]
    Song mainSong;
    [SerializeField, Tooltip("The 'Current Beatmap' variable in the scene.")]
    Beatmap currentBeatmap;
    
    // ================================================================
    // Internal variables
    // ================================================================

    // The instance of the currently-playing song.
    private FMOD.Studio.EventInstance instance;
    // The timeline handler tied to this music manager.
    public TimelineHandler handler;
    // Used to check if the instance is playing.
    FMOD.Studio.PLAYBACK_STATE playbackState;

    // ================================================================
    // Initializer and finalizer methods
    // ================================================================

    private void Awake()
    {
        // Set things up!
        // ================
        
        Debug.Assert(!mainSong.musicEvent.IsNull, "MusicManager Error: Start() failed. mainSong.musicEvent is null.");

        // Define the beatmap.
        currentBeatmap.Clear();
        currentBeatmap.Populate(mainSong.beatmapFile);

        // Create the timeline handler.
        instance = FMODUnity.RuntimeManager.CreateInstance(mainSong.musicEvent);
        handler = new TimelineHandler(instance);

        // DEBUG: Create a loop tracker.
        LoopTracker tracker = new LoopTracker(handler, currentBeatmap.length, 4);
        tracker.loopStart += Bong;
        tracker.batchStart += Bing;



#if UNITY_EDITOR
        // If we're in the editor, subscribe to the editor-pausing event.
        EditorApplication.pauseStateChanged += OnEditorPause;
#endif

    }

    private void Bing() { print("Bing!"); }
    private void Bong() { print("Bong!"); }

    private void Start()
    {
        // Starts our instance and our clock.
        // ================

        instance.start();
        handler.StartDSPClock(true);
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        // If we're in the editor, unsubscribe to the editor-pausing event.
        // ================
        
        EditorApplication.pauseStateChanged -= OnEditorPause;
    }
#endif

    // ================================================================
    // Pause / unpause methods
    // ================================================================

#if UNITY_EDITOR
    private void OnEditorPause(PauseState state)
    {
        // Detects when the editor pauses or plays. Passes a call to OnApplicationPause().
        // ================

        OnApplicationPause(state == PauseState.Paused);
    }
#endif

    private void OnApplicationPause(bool pauseStatus)
    {
        // Detects when the application pauses or plays. If we're paused, stop the timeline
        // handler from accumulating DSP Time.
        // ================

        if (pauseStatus)
        {
            instance.setPaused(true);
            handler.StopDSPClock();
        }
        else
        {
            instance.setPaused(false);
            handler.StartDSPClock(IsInstancePlaying());
        }
    }

    // ================================================================
    // Ongoing methods
    // ================================================================

    private void Update()
    {
        // Update the handler.
        // ================

        handler.Update();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        // If we're in the editor, display the timeline handler's GUI readout.
        // ================

        handler.OnGUI();
    }
#endif

    // ================================================================
    // Helper methods
    // ================================================================

    private bool IsInstancePlaying()
    {
        // Returns if an FMOD instance is currently playing.
        // ================

        instance.getPlaybackState(out playbackState);
        return playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

}
