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


        mainSong = songs[0];
    }
}