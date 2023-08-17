using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum BeatType { NONE, SingleSpawn, MassSpawn, GravityFlip }

[System.Serializable]
public class Beat 
{
    public uint number;
    public BeatType type;
}

[CreateAssetMenu(fileName="New Song", menuName="Song")]
public class songObject : ScriptableObject
{
    // ================================================================
    // Parameters
    // ================================================================

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

    // ================================================================
    // Internal variables
    // ================================================================

    // A dictionary object which is filled with the contents of nonNullBeats.
    public Dictionary<uint, BeatType> nonNullBeatsDict = new Dictionary<uint, BeatType>();

    // ================================================================
    // Data-accessor methods
    // ================================================================

    public BeatType GetBeatType( uint number )
    {
        // Accessor function used to return the beat type of a given beat, including
        // returning NONE if it is not in our dictionary.
        // ================

        if ( nonNullBeatsDict.ContainsKey(number) ) {
            return nonNullBeatsDict[number];
        }
        else {
            return BeatType.NONE;
        }
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    public void InitializeBeatsDict()
    {
        // Called from TimekeeperManager. Resets and repopulates our dictionary from our
        // nonNullBeats list.
        // ================

        nonNullBeatsDict.Clear();
        foreach ( Beat beat in nonNullBeats )
        {
            if ( !nonNullBeatsDict.ContainsKey(beat.number) ) {
                nonNullBeatsDict.Add(beat.number, beat.type);
                Debug.Log( $"Added ({beat.number}, {beat.type})" );
            }
        }
    }
}
