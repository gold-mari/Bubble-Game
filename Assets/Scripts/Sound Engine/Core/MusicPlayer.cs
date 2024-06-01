using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

public class MusicPlayer : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    // The timeline handler tied to this music manager.
    public TimelineHandler handler { get; protected set; }
    [SerializeField, Tooltip("The path of the FMOD music bus. Find it in the mixer by right clicking the Music group " +
                             "and selecting Copy Path.")]
    protected string musicBusPath = "bus:/Music";
    
    // ================================================================
    // Internal variables
    // ================================================================

    // The instance of the currently-playing song.
    protected FMODUnity.EventReference eventRef;
    // The instance of the currently-playing song.
    protected FMOD.Studio.EventInstance instance;
    // Used to check if the instance is playing.
    protected FMOD.Studio.PLAYBACK_STATE playbackState;
    // Used to save a restore the position for pausing, to counteract latency buildup.
    protected int timelinePosition;
    // Used to ensure we can only win once.
    protected bool songEnded = false;

    // ================================================================
    // Initializer and finalizer methods
    // ================================================================

    protected virtual void Awake()
    {
        // Set things up!
        // ================
        
        // Create the instance. If it's valid, create the timeline handler and continue.
        instance = FMODUnity.RuntimeManager.CreateInstance(eventRef);

        Debug.Assert(instance.isValid(), "MusicPlayer Error, Awake() failed. instance was not valid.", this);

        handler = new TimelineHandler(instance, musicBusPath);

#if UNITY_EDITOR
        // If we're in the editor, subscribe to the editor-pausing event.
        EditorApplication.pauseStateChanged += OnEditorPause;
#endif
    }

    protected virtual void Start()
    {
        // Starts our instance and our clock.
        // ================

        instance.start();
        handler.StartDSPClock(true);
    }

    private void OnDestroy()
    {
        // Stop the music when we're destroyed. Also, if we're in the editor, unsubscribe
        // to the editor-pausing event.
        // ================

        instance.release();
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

#if UNITY_EDITOR        
        EditorApplication.pauseStateChanged -= OnEditorPause;
#endif

    }

    // ================================================================
    // Pause / unpause / stop methods
    // ================================================================

#if UNITY_EDITOR
    protected void OnEditorPause(PauseState state)
    {
        // Detects when the editor pauses or plays. Passes a call to PauseMusic().
        // ================

        PauseMusic(state == PauseState.Paused);
    }
#endif

    protected void OnApplicationPause(bool pauseStatus)
    {
        // Detects when the application pauses or plays. If we're paused, stop the timeline
        // handler from accumulating DSP Time.
        // ================

        PauseMusic(pauseStatus);
    }

    public void PauseMusic(bool pauseStatus)
    {
        // Wrapper function for pausing and unpausing. Stops both our music and our DSP clock.
        // 
        // getTimelinePosition and setTimelinePosition are used to prevent latency buildup-
        // this solution courtesy of user emretanirgan from the FMOD forums:
        // https://qa.fmod.com/t/event-timeline-lags-behind-after-repeated-setpaused-calls/16694/3
        // ================

        if (pauseStatus)
        {
            instance.setPaused(true);
            instance.getTimelinePosition(out timelinePosition);
            handler.StopDSPClock();
        }
        else
        {
            instance.setPaused(false);
            instance.setTimelinePosition(timelinePosition);
            handler.StartDSPClock(IsInstancePlaying());
        }
    }

    protected void StopMusic(bool immediate=true)
    {
        FMOD.Studio.STOP_MODE stopMode = immediate ? 
                                         FMOD.Studio.STOP_MODE.IMMEDIATE : 
                                         FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
        instance.stop(stopMode);
        handler.StopDSPClock();
    }

    // ================================================================
    // Ongoing methods
    // ================================================================

    protected virtual void Update()
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