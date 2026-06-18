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

    public static Id GetStoryAchievement_ZERO_INDEX(int i)
    {
        if (i < 0 || i >= 5) {
            throw new System.ArgumentOutOfRangeException($"Index was not between 0 and 5: {i}");
        }

        return i switch {
            0 => Id.ACH_STORY_LVL1,
            1 => Id.ACH_STORY_LVL2,
            2 => Id.ACH_STORY_LVL3,
            3 => Id.ACH_STORY_LVL4,
            4 => Id.ACH_STORY_LVL5,
            _ => (Id)(-1)
        };
    }

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