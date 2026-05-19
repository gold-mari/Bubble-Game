using System.Collections.Generic;
using UnityEngine;

public interface ISteamAchievements
{
    public ISteamAchievements GetInstance()
    {
        Debug.LogError("ISteamAchievements Error: GetInstance was called directly.");
        return null;
    }

    public bool GetAchievement(string name, out bool achieved)
    {
        Debug.LogError("ISteamAchievements Error: GetAchievement was called directly.");
        achieved = false;
        return false;
    }

    public bool SetAchievement(string name)
    {
        Debug.LogError("ISteamAchievements Error: SetAchievement was called directly.");
        return false;
    }
}

public class AchievementsStub : ISteamAchievements
{
    private static AchievementsStub Instance = null;
    private Dictionary<string, bool> achievementDatabase = new(){
        {"ACH_STORY_LVL1",      false},
        {"ACH_STORY_LVL2",      false},
        {"ACH_STORY_LVL3",      false},
        {"ACH_STORY_LVL4",      false},
        {"ACH_STORY_LVL5",      false},
        {"ACH_SKILL_SRANK",     false},
        {"ACH_SKILL_STRAGGLER", false},
        {"ACH_SKILL_COMBO",     false},
        {"ACH_SKILL_LOSE",      false},
        {"ACH_SKILL_ENDLESS",   false},
        {"ACH_SKILL_HARDMODE",  false}
    };

    public static ISteamAchievements GetInstance()
    {
        // Lazily create an instance if we don't have one.

        Instance ??= new();
        return Instance;
    }

    public bool GetAchievement(string name, out bool achieved)
    {
        if (achievementDatabase.ContainsKey(name)) {
            achieved = achievementDatabase[name];
            Debug.Log($"AchievementStub: Achievement '{name}' is {(achieved ? "unlocked" : "NOT unlocked")}.");
            return true;
        } else {
            Debug.LogError($"AchievementsStub Error: Achievement '{name}' not found in database.");
            achieved = false;
            return false;
        }
    }

    public bool SetAchievement(string name)
    {
        if (achievementDatabase.ContainsKey(name)) {
            if (achievementDatabase[name] == true) {
                Debug.Log($"AchievementStub: Achievement '{name}' is already unlocked.");
            }
            Debug.Log($"AchievementStub: Unlocked achievement '{name}'.");
            achievementDatabase[name] = true;
            return true;
        } else {
            Debug.LogError($"AchievementsStub Error: Achievement '{name}' not found in database.");
            return false;   
        }
    }
}