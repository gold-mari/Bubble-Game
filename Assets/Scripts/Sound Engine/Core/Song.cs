using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu(fileName="New Song", menuName="Song")]
public class Song : ScriptableObject
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The music event associated with this song.")]
    public FMODUnity.EventReference musicEvent;
    [Tooltip("The text asset beatmap file associated with this song.")]
    public TextAsset beatmapFile;
}
