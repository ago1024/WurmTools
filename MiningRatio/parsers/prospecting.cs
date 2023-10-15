using System.Text.RegularExpressions;

public class ProspectingMessageParser : IMessageParser
{
    private readonly Regex reStart = new Regex("You start to gather fragments of the rock\\.|You start to analyse");
    private readonly Regex reEnd = new Regex("You find only rock\\.|You would mine (.* ore|.* shards) here\\.|You finish analysing");
    private readonly Regex reSkill = new Regex("Prospecting increased");

    public bool isActionStart(string message)
    {
        return reStart.IsMatch(message);
    }
    public bool isActionEnd(string message)
    {
        return reEnd.IsMatch(message);
    }
    public bool isSkillGain(string message)
    {
        return reSkill.IsMatch(message);
    }
    public string getName()
    {
        return "Prospecting";
    }
}

