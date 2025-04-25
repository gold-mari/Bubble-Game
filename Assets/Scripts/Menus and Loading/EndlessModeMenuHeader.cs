using UnityEngine;
using TMPro;
public class EndlessModeMenuHeader : MonoBehaviour
{
    public TMP_Text scoreText, timeText;
    public SaveHandler saveHandler;

    // Start is called before the first frame update
    void OnEnable()
    {
        RankStats stats = saveHandler.GetHighScore(6);

        if (stats == null || (stats.score == -1 && saveHandler.GetEndlessBestTime() == 0)) {
            scoreText.text = "N/A";
            timeText.text = "N/A";
        } else {
            scoreText.text = $"{stats.score}";
            System.TimeSpan time = System.TimeSpan.FromSeconds(saveHandler.GetEndlessBestTime());
            timeText.text = time.ToString(@"mm\:ss");
        }
    }
}
