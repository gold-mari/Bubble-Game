public class Achievement
{
    public enum Id
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

    public static string GetName(Id id) => System.Enum.GetName(typeof(Id), id);

    public static Id[] GetAchievementValues() => (Id[])System.Enum.GetValues(typeof(Id));

    // ==============================================================
    // Stats
    // ==============================================================

    public enum Stat
    {
        stat_Best_SRanks,
        stat_Best_Straggler,
        stat_Best_Combo
    }

    public static string GetName(Stat stat) => System.Enum.GetName(typeof(Stat), stat);

    public static Stat[] GetStatValues() => (Stat[])System.Enum.GetValues(typeof(Stat));
}