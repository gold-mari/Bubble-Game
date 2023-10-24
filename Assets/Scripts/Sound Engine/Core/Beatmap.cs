using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum BeatType { NONE, SingleSpawn, 
                       MassSpawn, 
                       FlipGravity,
                       MakeFlavorBomb,
                       HyperSpawn }

[CreateAssetMenu(fileName="New Beatmap", menuName="Beatmap")]
public class Beatmap : ScriptableObject
{
    // ================================================================
    // Data
    // ================================================================

    // The name of this beatmap, found in population. Used only for debug purposes.
    public string beatmapName { get; private set; }
    // The number of beats in the loop.
    public uint length { get; private set; }
    
    // A dictionary object which stores all beats with an associated type / event.
    private Dictionary<uint, BeatType> nonNullBeatsDict = new Dictionary<uint, BeatType>();

    // ================================================================
    // Data-accessor methods
    // ================================================================

    public BeatType GetBeatType(uint number)
    {
        // Accessor function used to return the beat type of a given beat, including
        // returning NONE if it is not in our dictionary.
        // ================

        Debug.Assert(number != 0, "Beatmap Error: GetBeatType() failed. Beatmaps use 1-indexing, meaning the first "
                                + "beat is beat 1, not beat 0.");

        try 
        {
            return nonNullBeatsDict[number];
        }
        catch 
        {
            return BeatType.NONE;
        }
    }

    // ================================================================
    // Data-manipulator methods
    // ================================================================

    public void Clear()
    {
        // Clears a beatmap.
        // ================

        nonNullBeatsDict.Clear();
    }

    public void Populate(TextAsset file)
    {
        // Populates beatmap from an external file. Beatmap files follow the format:

        // name,[NAME]
        // loopLength,[LOOP LENGTH]
        // singleSpawn,[VALID BEAT],[VALID BEAT],[ETC]
        // gravityFlip,[VALID BEAT],[VALID BEAT],[ETC]
        // massSpawn,...
        // makeFlavorBomb,...
        // hyperSpawn,...
        //
        // Such that each newline is its own field, and the first value on each line is the field
        // name. Fields 'name' and 'loopLength' are required, all others are optional. If an optional
        // field is not present in the file, that means no beats of that type are present. Whitespace
        // between values is trimmed.
        // ================

        nonNullBeatsDict.Clear();

        string[] lines = file.text.Split('\n');

        // We should have at least 2 lines: name and loopLength.
        Debug.Assert(lines.Length >= 2, "Beatmap Error: Populate() failed. File must have at least 2 lines.");

        // ================================================
        // LINE 1: Getting the beatmap name.
        // ================================================
        string[] nameValues = lines[0].Split(',');
        Debug.Assert(nameValues[0] == "name", "Beatmap Error: Populate() failed. First line must have key 'name'.");
        Debug.Assert(nameValues.Length == 2, "Beatmap Error: Populate() failed. First line 'name' must have exactly "
                                           + "1 key and 1 value.");
        beatmapName = nameValues[1];

        // ================================================
        // LINE 2: Getting the loop length.
        // ================================================
        string[] loopLengthValues = lines[1].Split(',');
        Debug.Assert(loopLengthValues[0] == "loopLength", "Beatmap Error: Populate() failed. Second line must have "
                                                        + "key 'loopLength'.");
        Debug.Assert(loopLengthValues.Length == 2, "Beatmap Error: Populate() failed. Second line 'loopLength' must "
                                                 + "have exactly 1 key and 1 value.");
        length = System.Convert.ToUInt32(loopLengthValues[1]);

        // ================================================
        // LINE 3: Parsing the beatmap.
        // ================================================
        // Start from the 3rd line and look onwards.
        for (int i = 2; i < lines.Length; i++)
        {
            string[] lineValues = lines[i].Split(',');
            BeatType type = BeatType.NONE; 

            foreach (string value in lineValues)
            {
                // Trim whitespace.
                string trimmed = value.Trim();

                if (trimmed == lineValues[0])
                {
                    type = TypeFromName(lineValues[0]);
                    Debug.Assert(type != BeatType.NONE, $"Beatmap Error: Populate() failed. Invalid type on line " 
                                                      + $"{i+1}: {lineValues[0]}.");
                }
                else
                {
                    // Add the (value, type) pair to our dictionary.
                    uint beatNumber = System.Convert.ToUInt32(trimmed);
                    Debug.Assert(beatNumber > 0 && beatNumber <= length, $"Beatmap Error: Populate() failed. "
                                                                       + $"Invalid beat number on line {i+1}: "
                                                                       + $"{beatNumber}.");
                    Debug.Assert(GetBeatType(beatNumber) == BeatType.NONE, $"Beatmap Error: Populate() failed. "
                                                                         + $"Beat number {beatNumber} on line {i+1} was "
                                                                         + $"already mapped.");
                    nonNullBeatsDict.Add(beatNumber, type);                    
                }
            }
        }
    }

    // ================================================================
    // Static methods
    // ================================================================

    private static BeatType TypeFromName(string name)
    {
        // Helper function for Populate. Returns a beat type, given that type's name.
        // ================

        if (name == "singleSpawn") return BeatType.SingleSpawn;
        if (name == "massSpawn") return BeatType.MassSpawn;
        if (name == "gravityFlip") return BeatType.FlipGravity;
        if (name == "makeFlavorBomb") return BeatType.MakeFlavorBomb;
        if (name == "hyperSpawn") return BeatType.HyperSpawn;
        else return BeatType.NONE;
    }
}