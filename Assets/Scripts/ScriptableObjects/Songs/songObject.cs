using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum BeatType { NONE, SingleSpawn, MassSpawn, GravityFlip }

[System.Serializable]
public class Beat 
{
    public int number;
    public BeatType type;
}

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

    [Tooltip("All beats with non-null actions.")]
    [ShowIf("showBeatInfo")]
    public List<Beat> nonNullBeats;
    [Tooltip("The next nonNullBeat coming up.")]
    [ShowIf("showBeatInfo")] [ReadOnly]
    public Beat upcomingBeat;

    private int index;

    public void Reset()
    {
        // Resets our position in our beat list.
        // ================

        index = 0;
        upcomingBeat = nonNullBeats[index];
    }

    public void Advance()
    {
        // Advances to the next position in our beat list.
        // ================

        if ( index < nonNullBeats.Count-1 ) {
            index++;
            upcomingBeat = nonNullBeats[index];
        }
    }
}
