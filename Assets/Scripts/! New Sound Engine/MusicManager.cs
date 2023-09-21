using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField, Tooltip("The main gameplay song to play in this scene.")]
    Song mainSong;
    [SerializeField, Tooltip("The 'Current Beatmap' variable in the scene.")]
    Beatmap currentBeatmap;

    // Start is called before the first frame update
    void Start()
    {
        currentBeatmap.Empty();
        currentBeatmap.Populate(mainSong.beatmapFile);
    }
}
