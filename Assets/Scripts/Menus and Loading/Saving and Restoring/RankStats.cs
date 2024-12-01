[System.Serializable]
public class RankStats
{
    public int stragglerBonus = -1;
    public int dangerBonus = -1;
    public int bubblesPopped = -1;
    public int maxCombo = -1;
    public int score = -1;
    public string rank = "NULL";

    public RankStats() {}

    public RankStats(RankStats other) {
        stragglerBonus = other.stragglerBonus;
        dangerBonus = other.dangerBonus;
        bubblesPopped = other.bubblesPopped;
        maxCombo = other.maxCombo;
        score = other.score;
        rank = other.rank;
    }

    public RankStats(
        int _stragglerBonus,
        int _dangerBonus,
        int _bubblesPopped,
        int _maxCombo,
        int _score,
        string _rank
    ) {
        stragglerBonus = _stragglerBonus;
        dangerBonus = _dangerBonus;
        bubblesPopped = _bubblesPopped;
        maxCombo = _maxCombo;
        score = _score;
        rank = _rank;
    }
}