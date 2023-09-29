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
    private Song mainSong;
    [SerializeField, Tooltip("The 'Current Beatmap' variable in the scene.")]
    private Beatmap currentBeatmap;
    [SerializeField, Tooltip("How long, in seconds, it takes us to tape stop on a loss.\n\nDefault: 2")]
    private float tapeStopDuration = 2;

    // The timeline handler tied to this music manager.
    public TimelineHandler handler { get; private set; }
    
    // ================================================================
    // Internal variables
    // ================================================================

    // The instance of the currently-playing song.
    private FMOD.Studio.EventInstance instance;
    // Used to check if the instance is playing.
    private FMOD.Studio.PLAYBACK_STATE playbackState;

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


#if UNITY_EDITOR
        // If we're in the editor, subscribe to the editor-pausing event.
        EditorApplication.pauseStateChanged += OnEditorPause;
#endif

    }

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
    // Pause / unpause / stop methods
    // ================================================================

#if UNITY_EDITOR
    private void OnEditorPause(PauseState state)
    {
        // Detects when the editor pauses or plays. Passes a call to OnApplicationPause().
        // ================

        PauseMusic(state == PauseState.Paused);
    }
#endif

    private void OnApplicationPause(bool pauseStatus)
    {
        // Detects when the application pauses or plays. If we're paused, stop the timeline
        // handler from accumulating DSP Time.
        // ================

        PauseMusic(pauseStatus);
    }

    public void PauseMusic(bool pauseStatus)
    {
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

    public void TapeStop()
    {
        // Applies the tape stop effect and ends the music. Called by endgame manager.
        // ================

        StartCoroutine(TapeStopRoutine());
    }
    private IEnumerator TapeStopRoutine()
    {
        // Applies the tape stop effect to the currently playing event. Also ends the music after.
        // ================

        handler.StopUpdating();

        float elapsed = 0;
        while (elapsed < tapeStopDuration)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TapeStop", (elapsed/tapeStopDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopMusic();
    }

    private void StopMusic()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        handler.StopDSPClock();
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

/*#if UNITY_EDITOR
    private void OnGUI()
    {
        // If we're in the editor, display the timeline handler's GUI readout.
        // ================

        handler.OnGUI();
    }
#endif*/

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
