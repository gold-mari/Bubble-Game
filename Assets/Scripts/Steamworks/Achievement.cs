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

    public static Id[] GetValues() => (Id[])System.Enum.GetValues(typeof(Id));
}