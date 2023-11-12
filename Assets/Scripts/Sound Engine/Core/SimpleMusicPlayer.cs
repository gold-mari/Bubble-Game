using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

public class SimpleMusicPlayer : MusicPlayer
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The music event to be played.")]
    private FMODUnity.EventReference musicEvent;

    // ================================================================
    // Initializer and finalizer methods
    // ================================================================

    protected override void Awake()
    {
        // Set things up!
        // ================
        
        eventRef = musicEvent;

        base.Awake();
    }
}