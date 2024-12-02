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
        "Cutscene_Level1", "Cutscene_Level2", "Cutscene_Level3", "Cutscene_Level4", "Cutscene_Level5", "Cutscene_Outro"
    };

    // ==============================================================
    // Saved fields
    // ==============================================================

    public class SaveData {
        public string lastPlayedScene = null;
        public bool playedBefore = false;
        public bool seenTutorial = false;
        public bool finishedGame = false;
        public RankStats[] highScores = new RankStats[5]{
            null, null, null, null, null
        };
    }
    private static SaveData saveData = null;

    // ==============================================================
    // Data-writing methods
    // ==============================================================

    private void Awake()
    {
        // saveData should (I think) be null once per game, when we have first opened the app.
        if (saveData == null) {
            // If it's null, load data from file.
            saveData = FileDataHandler.Load();
            if (saveData == null) {
                // If it's STILL null, make a new one!
                // print($"SaveHandler: No save found. Creating new struct.");
                saveData = new();
            } else {
                // print($"SaveHandler: Loaded data from file.");
            }
        }
    }

    private void Start()
    {
        // Start is called before the first frame update, ONCE per scene.
        // We use it to check the scene name- if it's a game scene, hold onto it.
        // ================

        string sceneName = SceneManager.GetActiveScene().name;

        // print($"SaveHandler: Current scene is {sceneName}");

        if (gameLevels.Contains(sceneName)) {
            // If it's a level, note the scene and note that we've played.
            saveData.lastPlayedScene = sceneName;
            saveData.playedBefore = true;
            // print($"SaveHandler: Saved lastPlayedScene and playedBefore");
        } else if (gameCutscenes.Contains(sceneName)) {
            // If it's a cutscene, just note the scene.
            saveData.lastPlayedScene = sceneName;
            // print($"SaveHandler: Saved lastPlayedScene");
        }

        Save();
    }

    public void SawTutorial()
    {
        saveData.seenTutorial = true;
        Save();
    }

    public void FinishedGame()
    {
        saveData.finishedGame = true;
        Save();
    }

    public bool TrySetHighScore(RankStats stats)
    {
        // Compares a rankStats against the high score for the current level.
        // If the new score is higher, set the new high score!
        // Returns whether or not it was a high score.
        // ================

        // If the game is not a level, throw an error.
        string sceneName = SceneManager.GetActiveScene().name;
        if (!gameLevels.Contains(sceneName)) {
            Debug.LogError($"SaveHandler Error: SetRankStats failed. Current scene ({sceneName}) is not a level.");
            return false;
        }

        // Before we go any further...
        // At this point in execution, we know that:
        //  * We're in a level
        //  * We have won, and are awaiting results.
        // In case of a crash, save our level as the NEXT one.
        saveData.lastPlayedScene = LevelLoader.Instance.QuerySceneDict("Next");
        Save();

        if (stats == null) {
            Debug.LogError($"SaveHandler Error: SetRankStats failed. stats was null.");
            return false;
        }

        // Find where the current scene is in our array, using it to index our highScores array.
        int index = Array.IndexOf(gameLevels, sceneName);
        
        // If the score is better, mark it as the new high score!
        if (saveData.highScores[index] == null || stats.score > saveData.highScores[index].score) {
            print($"SaveHandler: Saving high score into index {index}");
            saveData.highScores[index] = new RankStats(stats);
            Save();
            return true;
        }

        return false;
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

    // ==============================================================
    // Accessors
    // ==============================================================

    public string GetLastPlayedScene()
    {
        // Used to Continue the game.
        // ================

        if (saveData.lastPlayedScene == "") {
            return null;
        } else {
            return saveData.lastPlayedScene;
        }
    }

    public bool GetSeenTutorial()
    {   
        // Used to show / hide the tutorial badges.
        // ================

        return saveData.seenTutorial;
    }

    public bool GetPlayedBefore()
    {
        // Used to determine the scene we load after LoggerInit.
        // ================

        return saveData.playedBefore;
    }

    public bool GetFinishedGame()
    {
        // Used to show / hide our level select AND determine the scene we load after outro cutscene.
        // ================

        return saveData.finishedGame;
    }
}