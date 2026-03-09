using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    // ==============================================================
    // Data
    // ==============================================================

    public static readonly string[] gameLevels = new string[]{
        "Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "LevelE"
    };

    public static readonly string[] specialLevels =  new string[]{
        "Level6", "LevelE"
    };

    public static readonly string[] endlessLevels =  new string[]{
        "LevelE"
    };

    public static readonly string[] gameCutscenes = new string[]{
        "Cutscene_Level1", "Cutscene_Level2", "Cutscene_Level3", "Cutscene_Level4", "Cutscene_Level5", "Cutscene_Outro"
    };

    public static readonly string[] rankLookup = new string[]{
        "C", "B", "A", "S"
    };

    public static readonly string mainMenuScene = "MainMenu";

    // ==============================================================
    // Saved fields
    // ==============================================================

    public class SaveData {
        public string lastPlayedScene = null;
        public bool playedBefore = false;
        public bool seenTutorial = false;
        public bool finishedGame = false;
        public bool beatEndless = false;
        public bool playedLevel6 = false;
        public RankStats[] highScores = new RankStats[7]{
            null, null, null, null, null, null, null
        };
        public uint endlessBestTime = 0;
    }
    private static SaveData saveData = null;

    // ==============================================================
    // Data-writing methods
    // ==============================================================

    private void Awake()
    {
        // saveData should (I think) be null once per game session, when we have first opened the app.
        if (saveData == null) {
            LoadFresh();
        }
    }

    private void Start()
    {
        // Start is called before the first frame update, ONCE per scene.
        // We use it to check the scene name- if it's a game scene, hold onto it.
        // ================

        string sceneName = SceneManager.GetActiveScene().name;

        // print($"SaveHandler: Current scene is {sceneName}");

        if (gameLevels.Contains(sceneName) && !specialLevels.Contains(sceneName)) {
            // If it's a level, note the scene and note that we've played.
            saveData.lastPlayedScene = sceneName;
            saveData.playedBefore = true;
            // print($"SaveHandler: Saved lastPlayedScene and playedBefore");
        } else if (gameCutscenes.Contains(sceneName)) {
            // If it's a cutscene, just note the scene.
            saveData.lastPlayedScene = sceneName;
            // print($"SaveHandler: Saved lastPlayedScene");
        } else if (mainMenuScene.Equals(sceneName)) {
            LoadFresh();
        }

        Save();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
    //     if (GUI.Button(new Rect(70, 10, 50, 50), "HAS6"))
    //     {
    //         saveData.beatEndless = true;
    //         Save();
    //     }
    //     if (GUI.Button(new Rect(70, 70, 50, 50), "!PLAY6"))
    //     {
    //         saveData.playedLevel6 = false;
    //         Save();
    //     }
    //     if (GUI.Button(new Rect(140, 10, 50, 50), "RESET"))
    //     {
    //         saveData.endlessBestTime = 0;
    //         saveData.highScores[6] = null;
    //         Save();
    //     }
    //     if (GUI.Button(new Rect(140, 70, 50, 50), "GAME"))
    //     {
    //         saveData.finishedGame = true;
    //         Save();
    //     }
    }
#endif

    public void SawTutorial()
    {
        saveData.seenTutorial = true;
        Save();
    }

    public void FinishedGame()
    {
        return;
        saveData.finishedGame = true;
        Save();
    }

    public void BeatEndless()
    {
        return;
        saveData.beatEndless = true;
        Save();
    }

    public void PlayedLevel6()
    {
        return;
        saveData.playedLevel6 = true;
        Save();
    }

    public bool TrySetHighScore(RankStats stats)
    {
        // Compares a rankStats against the high score for the current level.
        // If the new score is higher, set the new high score!
        // Returns whether or not it was a high score.
        // ================

        return false;

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
        if (!specialLevels.Contains(sceneName)) {
            saveData.lastPlayedScene = LevelLoader.Instance.QuerySceneDict("Next");
            Save();
        }

        if (stats == null) {
            Debug.LogError($"SaveHandler Error: SetRankStats failed. stats was null.");
            return false;
        }

        // Find where the current scene is in our array, using it to index our highScores array.
        int index = Array.IndexOf(gameLevels, sceneName);
        // print($"SaveHandler: High Score --- old was {saveData.highScores[index].score}, new is {stats.score}.");

        string newRank = stats.rank;
        string oldRank = saveData.highScores[index].rank;
        bool rankIsBetter = rankLookup.Contains(newRank) && rankLookup.Contains(oldRank) &&
                            rankLookup.ToList().IndexOf(newRank) > rankLookup.ToList().IndexOf(oldRank);

        // If the score is better, mark it as the new high score!
        if (saveData.highScores[index] == null || stats.score > saveData.highScores[index].score || rankIsBetter) {
            print($"SaveHandler: Saving high score into index {index}.");
            saveData.highScores[index] = new RankStats(stats);
            Save();
            return true;
        }

        return false;
    }

    public bool TrySetBestTime(uint time)
    {
        // Compares a rankStats against the high score for the current level.
        // If the new score is higher, set the new high score!
        // Returns whether or not it was a high score.
        // ================

        // If the game is not a level, throw an error.
        string sceneName = SceneManager.GetActiveScene().name;
        if (!endlessLevels.Contains(sceneName)) {
            Debug.Log($"SaveHandler Notice: TrySetBestTime failed. Current scene ({sceneName}) is not an endless level.");
            return false;
        }

        // print($"SaveHandler: Best Time --- old was {saveData.endlessBestTime}, new is {time}.");

        // If the score is better, mark it as the new high score!
        if (time > saveData.endlessBestTime) {
            print($"SaveHandler: Saving best time. Old was {saveData.endlessBestTime}, new is {time}.");
            saveData.endlessBestTime = time;
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

        // SIX --- Do NOT SAVE data.

        FileDataHandler.Save(saveData);
    }

    public void Load()
    {
        // Loads our save data from our file, and is called when we first start
        // the game, in an initializer scene.
        // ================

        // If it's null, load data from file.
        saveData = FileDataHandler.Load();
        SaveData freshSave = new();
        if (saveData == null) {
            // If it's STILL null, make a new one!
            // print($"SaveHandler: No save found. Creating new struct.");
            saveData = freshSave;
        } else {
            // print($"SaveHandler: Loaded data from file.");

            // If we're missing high scores, add nulls until we have enough.
            // This is only relevant for if we add a level, like in the Endless Mode Update
            // when Level 6 and Level E were added.

            int scoresMissing = freshSave.highScores.Length-saveData.highScores.Length;
            if (scoresMissing > 0) {
                List<RankStats> scores = saveData.highScores.ToList();
                scores.AddRange(Enumerable.Repeat<RankStats>(null, scoresMissing));
                saveData.highScores = scores.ToArray();
            }

            // Debug.Log($"Extended high scores slots by {scoresMissing}. New length is {saveData.highScores.Length}");
        }
    }

    public void LoadFresh()
    {
        FileDataHandler.Save(null);
        Load();
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

    public bool GetBeatEndless()
    {
        // Used to show / hide our 6th level.
        // ================

        return saveData.beatEndless;
    }

    public bool GetPlayedLevel6()
    {
        // Used to show / hide the notif badge for our 6th level.
        // ================

        return saveData.playedLevel6;
    }

    public RankStats GetHighScore(int index)
    {
        return saveData.highScores[index];
    }

    public uint GetEndlessBestTime()
    {
        return saveData.endlessBestTime;
    }
}