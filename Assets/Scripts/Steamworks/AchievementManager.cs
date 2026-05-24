using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// Utility class for passing info between SaveData and Steamworks.
/// The goal is to ALWAYS keep the two in sync.
/// </summary>
public class AchievementManager : MonoBehaviour {
    public Achievement.Id queried_Id;
    public Achievement.Stat queried_Stat;
    public int queried_Value;
    public SaveHandler saveHandler;
    [Expandable]
    public AchievementsStub stub;
    
    
    
    private ISteamAchievements STEAM_ACHIEVEMENT_INTERFACE;

    public void Awake()
    {
        STEAM_ACHIEVEMENT_INTERFACE = stub;
        // STEAM_ACHIEVEMENT_INTERFACE = SteamManager.GetInstance();
    }

    public void Start()
    {
        if (saveHandler) {
            saveHandler.UnlockedAchievement += WriteAchievement;
            saveHandler.SyncSaveToOnline += () => {
                DownloadOnlineAchievements();
                DownloadOnlineStats();
            };
        }

        if (SteamManager.Initialized) {
            UploadOfflineStats();
            UploadOfflineAchievements();
            DownloadOnlineStats();
            DownloadOnlineAchievements();
        }
    }

    // ==============================================================
    // SERVER-SIDE reading
    // ==============================================================

    public int ReadStat(Achievement.Stat stat)
    {
        // Returns:
        // * [anything else] --- value of stat
        // * -1              --- stat does not exist
        // * -2              --- SteamManager is not initialized
        //
        // TODO: Change these sentinel values if we start using stats that can be negative.
        // ================

        if(SteamManager.Initialized && STEAM_ACHIEVEMENT_INTERFACE != null) {
            bool success = STEAM_ACHIEVEMENT_INTERFACE.GetStat(Achievement.GetName(stat), out int value);

            if (success) return value;
            else return -1;
        } 

        else return -2;
    }

    public int ReadAchievement(Achievement.Id id)
    {
        // Returns:
        // * 1  --- achievement is unlocked
        // * 0  --- achievement is not unlocked
        // * -1 --- achievement does not exist
        // * -2 --- SteamManager is not initialized
        // ================

        if(SteamManager.Initialized && STEAM_ACHIEVEMENT_INTERFACE != null) {
            bool success = STEAM_ACHIEVEMENT_INTERFACE.GetAchievement(Achievement.GetName(id), out bool achieved);

            if (success) return achieved ? 1 : 0;
            else return -1;
        } 

        return -2;
    }

    // ==============================================================
    // SERVER-SIDE writing
    // ==============================================================

    public void WriteAchievement(Achievement.Id id)
    {
        if(SteamManager.Initialized) {
            STEAM_ACHIEVEMENT_INTERFACE.SetAchievement(Achievement.GetName(id));
        }
    }

    public void WriteStat(Achievement.Stat stat, int value)
    {
        if(SteamManager.Initialized) {
            STEAM_ACHIEVEMENT_INTERFACE.SetStat(Achievement.GetName(stat), value);
        }
    }

    // ==============================================================
    // Sync methods
    // ==============================================================   

    public void DownloadOnlineAchievements()
    {
        // Checks each achievement.
        // If it is unlocked on Steam but not in our save, set it in our save.
        // ================

        if (saveHandler) {
            foreach (Achievement.Id id in Achievement.GetAchievementValues()) {
                // If the achievement was unlocked on Steamworks (like for stat-based achievements 
                // that are automatically unlocked), but it's not unlocked in our save...
                if (ReadAchievement(id) == 1 && !saveHandler.GetUnlockedAchievement(id)) {
                    Debug.Log($"AchievementManager: Downloading online achievement {id}.");
                    // Add the achievement to our save.
                    saveHandler.TrySetAchievement(id);
                }
            }
        }
    }

    public void UploadOfflineAchievements()
    {
        // Checks each achievement.
        // If it is unlocked in our save but not on Steam, set it on Steam.
        // ================

        if (saveHandler) {
            foreach (Achievement.Id id in Achievement.GetAchievementValues()) {
                // If the achievement was unlocked offline, and we are now online...
                if (saveHandler.GetUnlockedAchievement(id) && ReadAchievement(id) == 0) {
                    // Write the achievement to Steamworks.
                    Debug.Log($"AchievementManager: Uploading offline achievement {id}.");
                    WriteAchievement(id);
                }
            }
        }
    }

    public void DownloadOnlineStats()
    {
        // Checks each stat.
        // If it is higher on Steam than it is in our save, update our save to have the higher value.
        //
        // This shouldn't really happen unless you nuke your save.
        // ================

        if (saveHandler) {
            foreach (Achievement.Stat stat in Achievement.GetStatValues()) {
                int onlineValue = ReadStat(stat);
                int offlineValue = saveHandler.GetStat(stat);
                // If the stat got nuked offline, and we're not getting an error code while reading Steam...
                if (offlineValue < onlineValue && onlineValue >= 0) {
                    Debug.Log($"AchievementManager: Downloading online stat {stat}.");
                    // Update the stat in our save.
                    saveHandler.TrySetStat(stat, onlineValue);
                }
            }
        }
    }

    public void UploadOfflineStats()
    {
        // Checks each stat.
        // If it is higher in our save than it is on Steam, update Steam to have the higher value.
        //
        // If the player has cracked the save cipher and cheats this way, fair play to them, I guess.
        // ================

        if (saveHandler) {
            foreach (Achievement.Stat stat in Achievement.GetStatValues()) {
                int onlineValue = ReadStat(stat);
                int offlineValue = saveHandler.GetStat(stat);
                // If the stat got better offline, and we're not getting an error code while reading Steam...
                if (offlineValue > onlineValue && onlineValue >= 0) {
                    Debug.Log($"AchievementManager: Uploading offline stat {stat}.");
                    // Write the new, greater stat to Steamworks.
                    WriteStat(stat, offlineValue);
                }
            }
        }
    }

    public void DEBUG_FORCE_SYNC()
    {
        UploadOfflineStats();
        UploadOfflineAchievements();

        DownloadOnlineStats();
        DownloadOnlineAchievements();
    }

    // ==============================================================
    // Debug / testing methods
    // ==============================================================   

    [Button] public void UnlockAchievement() => saveHandler.TrySetAchievement(queried_Id);
    [Button] public void SetStat() => saveHandler.TrySetStat(queried_Stat, queried_Value);
    [Button] public void FORCE_SYNC() => DEBUG_FORCE_SYNC();
}