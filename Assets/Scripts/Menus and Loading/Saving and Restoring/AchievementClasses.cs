[System.Serializable]
public class SerializedSteamAchievement
{
    public Achievement.Id id;
    public bool value;
    public SerializedSteamAchievement(Achievement.Id _id, bool _value){ id = _id; value = _value; }
}

[System.Serializable]
public class SerializedSteamStat
{
    public Achievement.Stat id;
    public int value;
    public SerializedSteamStat(Achievement.Stat _id, int _value){ id = _id; value = _value; }
}