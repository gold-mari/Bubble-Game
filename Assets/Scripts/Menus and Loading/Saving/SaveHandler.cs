using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    // ==============================================================
    // Data
    // ==============================================================

    private static readonly string[] gameLevels = new string[]{
        "Level1", "Level2", "Level3", "Level4", "Level5"
    };

    private static readonly string[] gameCutscenes = new string[]{
        "Cutscene_Level1", "Cutscene_Level2", "Cutscene_Level3", "Cutscene_Level4", "Cutscene_Level5"
    };

    // ==============================================================
    // Saved fields
    // ==============================================================

    private static string lastPlayedScene = "NULL";
    private static RankStats[] highScores = new RankStats[5]{
        null, null, null, null, null
    };

    // ==============================================================
    // Data-writing methods
    // ==============================================================

    private void Start()
    {
        // Start is called before the first frame update, ONCE per scene.
        // We use it to check the scene name- if it's a game scene, hold onto it.
        // ================

        string sceneName = SceneManager.GetActiveScene().name;
        if (gameLevels.Contains(sceneName) || gameCutscenes.Contains(sceneName)) {
            lastPlayedScene = sceneName;
            Save();
        }
    }

    public void SetRankStats(RankStats stats)
    {
        // If the game is not a level...
        string sceneName = SceneManager.GetActiveScene().name;
        if (!gameLevels.Contains(sceneName)) {
            Debug.LogError($"SaveHandler Error: SetRankStats failed. Current scene ({sceneName}) is not a level.");
        }

        int index = Array.IndexOf(gameLevels, sceneName);
        
        // If the score is better, mark it as the new high score!
        if (stats.score > highScores[index].score) {
            highScores[index] = stats;
            Save();
        }
    }

    // ==============================================================
    // Save/Load methods
    // ==============================================================

    public void Save()
    {
        // Save writes our save data to our file, and is called every time one of
        // our saved fields changes.
        // ================
    }

    public void Load()
    {
        // Loads our save data from our file, and is called when we first start
        // the game, in an initializer scene.
        // ================
    }
}