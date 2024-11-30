using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor.EditorTools;
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

    public class SaveData {
        public string lastPlayedScene = "NULL";
        public RankStats[] highScores = new RankStats[5]{
            null, null, null, null, null
        };
        public bool playedBefore = false;
    }
    private static SaveData saveData;

    // ==============================================================
    // Data-writing methods
    // ==============================================================

    private void Start()
    {
        // Start is called before the first frame update, ONCE per scene.
        // We use it to check the scene name- if it's a game scene, hold onto it.
        // ================

        string sceneName = SceneManager.GetActiveScene().name;

        if (gameLevels.Contains(sceneName)) {
            // If it's a level, note the scene and note that we've played.
            saveData.lastPlayedScene = sceneName;
            saveData.playedBefore = true;
            Save();
        } else if (gameCutscenes.Contains(sceneName)) {
            // If it's a cutscene, just note the scene.
            saveData.lastPlayedScene = sceneName;
            Save();
        }
    }

    public void TrySetHighScore(RankStats stats)
    {
        // Compares a rankStats against the high score for the current level.
        // If the new score is higher, set the new high score!
        // ================

        // If the game is not a level, throw an error.
        string sceneName = SceneManager.GetActiveScene().name;
        if (!gameLevels.Contains(sceneName)) {
            Debug.LogError($"SaveHandler Error: SetRankStats failed. Current scene ({sceneName}) is not a level.");
            return;
        }

        // Find where the current scene is in our array, using it to index our highScores array.
        int index = Array.IndexOf(gameLevels, sceneName);
        
        // If the score is better, mark it as the new high score!
        if (stats.score > saveData.highScores[index].score) {
            saveData.highScores[index] = stats;
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

        FileDataHandler.Save(saveData);
    }

    public void Load()
    {
        // Loads our save data from our file, and is called when we first start
        // the game, in an initializer scene.
        // ================

        SaveData loadedData = FileDataHandler.Load();
        // LoadedData if nonnull, else new SaveData.
        saveData = loadedData ?? new SaveData();
    }
}