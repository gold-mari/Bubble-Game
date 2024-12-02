using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelSelectHeader : MonoBehaviour
{
    public TMP_Text title, subtitle;
    public SaveHandler saveHandler;

    // Start is called before the first frame update
    void OnEnable()
    {
        title.text = "Select a Level!";
        subtitle.text = "Your high score will be listed here.";
    }

    public void ShowCutscene(string id)
    {
        title.text = id;
        subtitle.text = "No high score for this stage.";
    }

    public void ShowStats(int levelIndex)
    {
        RankStats stats = saveHandler.GetHighScore(levelIndex);
        title.text = $"Level {levelIndex+1}";
        if (stats.score == -1) {
            subtitle.text = $"Level has not yet been won!";
        } else {
            subtitle.text = $"High Score: {stats.score} - Rank: {stats.rank}";
        }
    }
}
