using UnityEngine;
using System.Collections;
using Steamworks;
using System;
using NaughtyAttributes;

public class AchievementManager : MonoBehaviour {
    
    public enum Achievement_Id
    {
        ACH_STORY_LVL1,
        ACH_STORY_LVL2,
        ACH_STORY_LVL3,
        ACH_STORY_LVL4,
        ACH_STORY_LVL5,
        ACH_SKILL_SRANK,
        ACH_SKILL_STRAGGLER,
        ACH_SKILL_COMBO,
        ACH_SKILL_LOSE,
        ACH_SKILL_ENDLESS,
        ACH_SKILL_HARDMODE
    }
    
    public Achievement_Id achievement_Id;

    void Start() {
		if(SteamManager.Initialized) {
			string name = SteamFriends.GetPersonaName();
			Debug.Log(name);
		}
	}

    public void GetAchievementStatus(Achievement_Id id)
    {
        string achievementName = Enum.GetName(typeof(Achievement_Id), id);
        bool success = SteamUserStats.GetAchievement(achievementName, out bool achieved);
        if (success) Debug.Log($"Achieved: {achieved}");
        else Debug.Log($"Achievement not found!");
    }

    [Button]
    public void QueryAchievement()
    {
        GetAchievementStatus(achievement_Id);
    }
}