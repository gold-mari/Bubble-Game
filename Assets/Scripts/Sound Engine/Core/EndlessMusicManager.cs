using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class EndlessMusicManager : MusicManager
{
    // ================================================================
    // Parameters
    // ================================================================

    [Expandable, SerializeField, Tooltip("The songs to play in this scene.")]
    private Song[] songs;

    public System.Action OnNextSong;

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
        mainSong = songs[_index];

        base.Awake();
    }

    protected override void OnMarkerUpdated(string lastMarker)
    {
        if (lastMarker == "endlessNextSong") {
            StopMusic(false);

            OnNextSong?.Invoke();

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

        if (_index >= songs.Length) {
            _index = 0;
            _semitoneOffset += 1;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SemitoneOffset", _semitoneOffset);
        }

        mainSong = songs[_index];
        return InitializeSong();
    }
}