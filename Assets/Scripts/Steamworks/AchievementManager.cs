using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// Utility class for reading from the SaveData and writing to Steamworks.
/// </summary>
public class AchievementManager : MonoBehaviour {
    public Achievement.Id queried_Id;
    public SaveHandler saveHandler;
    
    
    
    private ISteamAchievements STEAM_ACHIEVEMENT_INTERFACE;


    public void Start()
    {
        if (saveHandler) {
            saveHandler.UnlockedAchievement += WriteAchievement;
        }

        if (SteamManager.Initialized) {
            WriteAllAchievements();

            STEAM_ACHIEVEMENT_INTERFACE = AchievementsStub.GetInstance();
            // STEAM_ACHIEVEMENT_INTERFACE = SteamManager.GetInstance();
        }
    }

    public int ReadAchievementStatus(Achievement.Id id)
    {
        // Returns:
        // * 1  --- achievement is unlocked
        // * 0  --- achievement is not unlocked
        // * -1 --- achievement does not exist
        // * -2 --- SteamManager is not initialized
        // ================

        if(SteamManager.Initialized) {
            bool success = STEAM_ACHIEVEMENT_INTERFACE.GetAchievement(Achievement.GetName(id), out bool achieved);

            if (success) return achieved ? 1 : 0;
            else return -1;
        } 

        return -2;
    }

    public void WriteAchievement(Achievement.Id id)
    {
        if(SteamManager.Initialized) {
            STEAM_ACHIEVEMENT_INTERFACE.SetAchievement(Achievement.GetName(id));
        }
    }

    public void WriteAllAchievements()
    {
        if (saveHandler) {
            foreach (Achievement.Id id in Achievement.GetValues()) {
                if (saveHandler.GetUnlockedAchievement(id) && ReadAchievementStatus(id) == 0) {
                    WriteAchievement(id);
                }
            }
        }
    }

    [Button] public void Query() => ReadAchievementStatus(queried_Id);
    [Button] public void Unlock() => WriteAchievement(queried_Id);
}