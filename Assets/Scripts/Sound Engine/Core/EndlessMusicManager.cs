using UnityEngine;
using NaughtyAttributes;

public class EndlessMusicManager : MusicManager
{
    // ================================================================
    // Helper structs
    // ================================================================

    [System.Serializable]
    public struct EndlessStage
    {
        [Expandable]
        public Song song;
        public uint ringSize;
    }

    // ================================================================
    // Parameters
    // ================================================================

    [Header("Endless Parameters")]
    [SerializeField, Tooltip("The BubbleSpawner in this stage (for setting mass spawn sizes).")]
    private BubbleSpawner spawner;
    [SerializeField, Tooltip("The stages to cycle through in this scene.")]
    private EndlessStage[] stages;

    // ================================================================
    // Misc Internal Variables
    // ================================================================
    
    private int _index = 0;
    private float _semitoneOffset = 0;

    // ================================================================
    // Initializers
    // ================================================================

    protected override void Awake()
    {
        _index = 0;
        mainSong = stages[_index].song;
        spawner.SetMassRoundSize(stages[_index].ringSize);

        base.Awake();
    }

    protected override void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("EndlessMode", 1);
        base.Start();
    }

    protected override void OnMarkerUpdated(string lastMarker)
    {
        Debug.Log($"EndlessMusicManager.OnMarkerUpdated - {lastMarker}");

        if (lastMarker == "endlessNextSong") {
            StopMusic(false);

            TimelineHandler newHandler = NextSong();
            handler.PassAllSubscribersTo(newHandler);
            handler = newHandler;

            Begin();
        }
        
        if (lastMarker != "end") { // Intercept the song ending. This is ENDLESS mode <3
            base.OnMarkerUpdated(lastMarker);
        }
    }

    private TimelineHandler NextSong()
    {
        _index++;

        if (_index >= stages.Length) {
            _index = 0;
            _semitoneOffset += 1;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SemitoneOffset", _semitoneOffset);
        }

        mainSong = stages[_index].song;
        spawner.SetMassRoundSize(stages[_index].ringSize);

        // Returns the TimelineHandler produced by initializing the new FMOD event.
        return InitializeSong();
    }
}