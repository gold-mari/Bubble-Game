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

    public bool GetStat(string name, out int value)
    {
        Debug.LogError("ISteamAchievements Error: GetStat was called directly.");
        value = -1;
        return false;
    }

    public bool SetStat(string name, int value, int max)
    {
        Debug.LogError("ISteamAchievements Error: SetStat was called directly.");
        return false;
    }

    public void NUKE_EVERYTHING(bool areUSure=false, bool areUReallySure=false, bool areUReallyReallySure=false)
    {
        Debug.LogError("ISteamAchievements Error: NUKE_EVERYTHING was called directly.");
    }
}