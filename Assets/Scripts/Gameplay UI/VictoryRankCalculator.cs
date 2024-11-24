using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VictoryRankCalculator : MonoBehaviour
{
    [SerializeField, Tooltip("The TextAsset containing the thresholds for ranks.")]
    private TextAsset rankDocument;
    [SerializeField, Tooltip("The uintVar containing our score.")]
    private uintVar scoreVar;

    private List<RankEntry> rankData = null;
    private string id = "undefined";

    private void Start()
    {
        InitializeRanks(rankDocument.text);
    }

    private void InitializeRanks(string text)
    {
        rankData = new();

        string[] lines = text.Split('\n');

        foreach (string line in lines) {
            // Ignore comments.
            if (line.StartsWith("//")) continue;

            string[] tokens = line.Split(',');
            if (tokens.Length == 1) {
                id = tokens[0];
            } else if (tokens.Length == 2) {
                RankEntry entry = new(tokens[1], int.Parse(tokens[0]));
                rankData.Add(entry);
            }
        }

        rankData.OrderBy(entry => entry.lowBound);
    }

    public string CalculateRank()
    {
        if (rankData == null) {
            // rankData has not been initialized.
            Debug.LogError("VictoryRankCalculator Error: rankData list has not been initialized.");
            return "NULL";
        }

        for (int i = 0; i < rankData.Count; i++) {
            if (scoreVar.value < rankData[i].lowBound) { // If we're under the bound of the current rank...
                if (i > 0) { // And we're not the lowest rank...
                    return rankData[i-1].rank; // Return that rank.
                } else { // Somehow we scored below a C.
                    return "FAIL";
                }
            }
        }

        // If we made it here, we're in the top rank!
        return rankData[^1].rank;
    }

    private struct RankEntry
    {
        public string rank;
        public int lowBound;

        public RankEntry(string _rank, int _lowBound)
        {
            rank = _rank;
            lowBound = _lowBound;
        }
    }
}