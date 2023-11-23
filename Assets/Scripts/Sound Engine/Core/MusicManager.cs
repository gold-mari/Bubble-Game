using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

public class MusicManager : MusicPlayer
{
    // ================================================================
    // Parameters
    // ================================================================

    [Expandable, SerializeField, Tooltip("The main gameplay song to play in this scene.")]
    private Song mainSong;
    [SerializeField, Tooltip("The 'Current Beatmap' variable in the scene.")]
    private Beatmap currentBeatmap;
    [SerializeField, Tooltip("The floatVar representing how far we are into the song.")]
    private floatVar songCompletion;
    [SerializeField, Tooltip("How long, in seconds, it takes us to tape stop on a loss.\n\nDefault: 2")]
    private float tapeStopDuration = 2;
    [SerializeField, Tooltip("The endgame manager present in this scene.")]
    private EndgameManager endgameManager;

    // ================================================================
    // Initializer and finalizer methods
    // ================================================================

    protected override void Awake()
    {
        // Set things up!
        // ================
        
        Debug.Assert(!mainSong.musicEvent.IsNull, "MusicManager Error: Start() failed. mainSong.musicEvent is null.");

        // Define the beatmap.
        currentBeatmap.Clear();
        if (!mainSong.isMedley)
        {
            currentBeatmap.Populate(mainSong.beatmapFile);
        }
        else // if (mainSong.isMedley)
        {
            currentBeatmap.Populate(mainSong.beatmapFiles[0].beatmapFile);
        }

        eventRef = mainSong.musicEvent;

        // Call our base awake function, which includes creating our timeline handler.
        base.Awake();

        // Subscribe to the markerUpdated function. Used to know when the song ends.
        handler.markerUpdated += OnMarkerUpdated;
    }

    protected override void Start()
    {
        // Starts our instance and our clock.
        // ================

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TapeStop", 0);
        //songCompletion.value = 0;
        base.Start();
    }

    // ================================================================
    // Update methods
    // ================================================================

    protected override void Update()
    {
        // Update is called once per frame. We use it to update our songCompletion floatVar.
        // ================

        base.Update();
        songCompletion.value = (float)handler.DSPTime/handler.musicLength;
    }

    // ================================================================
    // Pause / unpause / stop methods
    // ================================================================

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
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TapeStop", elapsed/tapeStopDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopMusic();
    }

    // ================================================================
    // Event handling methods
    // ================================================================

    private void OnMarkerUpdated(string lastMarker)
    {
        if (lastMarker == "end" && !songEnded) {
            songEnded = true;
            endgameManager.TriggerWin();
        }
        else if (mainSong.isMedley)
        {
            string[] medleyStrings = lastMarker.Split('-');
            if (medleyStrings[0] == "switchMap")
            {
                Debug.Assert(medleyStrings.Length == 2, $"MusicManager error: OnMarkerUpdated failed. "
                                                      + $"Unable to parse switchMap marker: {lastMarker}");
                foreach (MedleyBeatmap map in mainSong.beatmapFiles)
                {
                    if (map.beatmapName == medleyStrings[1])
                    {
                        currentBeatmap.Clear();
                        currentBeatmap.Populate(map.beatmapFile);
                        return;
                    }
                }
            }
        }
    }
}