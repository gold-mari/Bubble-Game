using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementsStub")]
public class AchievementsStub : ScriptableObject, ISteamAchievements
{
    // ==============================================================
    // Helper classes
    // ==============================================================

    [System.Serializable]
    public class StringBoolPair {
        public string s; public bool b; 
        public StringBoolPair(string S, bool B) { s=S; b=B; }
    }

    [System.Serializable]
    public class StringIntPair {
        public string s; public int i; 
        public StringIntPair(string S, int I) { s=S; i=I; }
    }

    // ==============================================================
    // Data
    // ==============================================================

    private static AchievementsStub Instance = null;
    public List<StringBoolPair> achievementDatabase = new(){
        new("ACH_STORY_LVL1",      false),
        new("ACH_STORY_LVL2",      false),
        new("ACH_STORY_LVL3",      false),
        new("ACH_STORY_LVL4",      false),
        new("ACH_STORY_LVL5",      false),
        new("ACH_SKILL_SRANK",     false),
        new("ACH_SKILL_STRAGGLER", false),
        new("ACH_SKILL_COMBO",     false),
        new("ACH_SKILL_LOSE",      false),
        new("ACH_SKILL_ENDLESS",   false),
        new("ACH_SKILL_HARDMODE",  false)
    };
    public List<StringIntPair> statsDatabase = new(){
        new("stat_Best_SRanks",    0),
        new("stat_Best_Straggler", 0),
        new("stat_Best_Combo",     0)
    };

    // ==============================================================
    // Interface methods
    // ==============================================================

    public static ISteamAchievements GetInstance()
    {
        // Lazily create an instance if we don't have one.
        Instance ??= new();
        return Instance;
    }

    public bool GetAchievement(string name, out bool achieved)
    {
        var target = achievementDatabase.FirstOrDefault(p => p.s == name);
        if (target !=  null) {
            achieved = target.b;
            // Debug.Log($"AchievementStub: Achievement '{name}' is {(achieved ? "unlocked" : "NOT unlocked")}.");
            return true;
        } else {
            // Debug.LogError($"AchievementsStub Error: Achievement '{name}' not found in database.");
            achieved = false;
            return false;
        }
    }

    public bool SetAchievement(string name)
    {
        var target = achievementDatabase.FirstOrDefault(p => p.s == name);
        if (target !=  null) {
            if (target.b == true) {
                // Debug.Log($"AchievementStub: Achievement '{name}' is already unlocked.");
            } else {
                // Debug.Log($"AchievementStub: Unlocked achievement '{name}'.");  
            }

            target.b = true;
            return true;
        } else {
            // Debug.LogError($"AchievementsStub Error: Achievement '{name}' not found in database.");
            return false;   
        }
    }

    public bool GetStat(string name, out int value)
    {
        var target = statsDatabase.FirstOrDefault(p => p.s == name);
        if (target != null) {
            value = target.i;
            // Debug.Log($"AchievementStub: Stat '{name}' is {value}.");
            return true;
        } else {
            // Debug.LogError($"AchievementsStub Error: Stat '{name}' not found in database.");
            value = -1;
            return false;
        }
    }

    public bool SetStat(string name, int value)
    {
        var target = statsDatabase.FirstOrDefault(p => p.s == name);
        if (target != null) {
            target.i = value;
            // Debug.Log($"AchievementStub: Set stat '{name}' to {target.i}.");
            return true;
        } else {
            // Debug.LogError($"AchievementsStub Error: Stat '{name}' not found in database.");
            return false;   
        }
    }
}