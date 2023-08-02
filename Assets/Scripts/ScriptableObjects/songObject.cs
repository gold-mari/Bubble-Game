using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName="New Song", menuName="Song")]
public class songObject : ScriptableObject
{
    [Tooltip("The music event associated with this song.")]
    public FMODUnity.EventReference musicEvent;
    [Tooltip("Whether or not we should display beat information.")]
    [SerializeField]
    private bool showBeatInfo;
    [Tooltip("The number of beats per event loop. 1-indexed: the first beat is beat 1.")]
    [ShowIf("showBeatInfo")]
    public uint loopLength;
    [Tooltip("The beats on which we should spawn a bubble.")]
    [ShowIf("showBeatInfo")]
    public List<uint> spawnBeats;
    [Tooltip("The beats on which we should flip gravity.")]
    [ShowIf("showBeatInfo")]
    public List<uint> flipBeats;
    [Tooltip("The beats on which we should spawn a mass round.")]
    [ShowIf("showBeatInfo")]
    public List<uint> massBeats;
}
