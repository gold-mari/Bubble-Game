using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class MedleyBeatmap
{
    public string beatmapName;
    public TextAsset beatmapFile;
}

[CreateAssetMenu(fileName="New Song", menuName="Song")]
public class Song : ScriptableObject
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The music event associated with this song.")]
    public FMODUnity.EventReference musicEvent;
    [Tooltip("Whether or not this music event represents a medley of beatmaps.")]
    public bool isMedley = false;
    [HideIf("isMedley"), Tooltip("The text asset beatmap file associated with this song.")]
    public TextAsset beatmapFile;
    [ShowIf("isMedley"), Tooltip("The text asset beatmap files associated with this medley.")]
    public MedleyBeatmap[] beatmapFiles;
}
