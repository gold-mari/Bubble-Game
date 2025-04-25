using UnityEngine;
using UnityEngine.Events;
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
        public bool switchColorOnMap;
        public Color baseColor, orbColor;
    }

    // ================================================================
    // Parameters
    // ================================================================

    [Header("Endless Parameters")]
    [SerializeField, Tooltip("The uintVar representing how long we've been alive for, in seconds.")]
    private uintVar secondsAlive;
    [SerializeField, Tooltip("The BubbleSpawner in this stage (for setting mass spawn sizes).")]
    private BubbleSpawner spawner;
    [SerializeField, Tooltip("The stages to cycle through in this scene.")]
    private EndlessStage[] stages;

    // "Beating Endless Mode" refers to clearing all stages once. 
    // IE, this action is called whenever we loop back to the start.
    public UnityEvent OnBeatEndless;
    public System.Action<EndlessStage> OnNextStage;

    // ================================================================
    // Misc Internal Variables
    // ================================================================
    
    private int _index = 0;
    private float _semitoneOffset = 0;
    private double _timeAliveDouble = 0;
    private bool _accumulateTimeAlive = false;

    // ================================================================
    // Initializers
    // ================================================================

    protected override void Awake()
    {
        _index = 0;
        _timeAliveDouble = secondsAlive.value = 0;

        mainSong = stages[_index].song;
        spawner.SetMassRoundSize(stages[_index].ringSize);

        base.Awake();
    }

    protected override void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("EndlessMode", 1);
        OnNextStage?.Invoke(stages[_index]);

        base.Start();
        _accumulateTimeAlive = true;
    }

    // ================================================================
    // Update methods
    // ================================================================

    protected override void Update()
    {
        base.Update();
        if (_accumulateTimeAlive) {
            _timeAliveDouble += Time.deltaTime;
            secondsAlive.value = System.Convert.ToUInt32(_timeAliveDouble);
        }
    }

    // ================================================================
    // Controls methods
    // ================================================================

    protected override void OnMarkerUpdated(string lastMarker)
    {
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

            OnBeatEndless?.Invoke();
        }

        mainSong = stages[_index].song;
        spawner.SetMassRoundSize(stages[_index].ringSize);
        OnNextStage?.Invoke(stages[_index]);

        // Returns the TimelineHandler produced by initializing the new FMOD event.
        return InitializeSong();
    }

    public void StopAccumulatingTimeAlive()
    {
        _accumulateTimeAlive = false;
    }
}