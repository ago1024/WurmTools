using System.Text.RegularExpressions;

public class RopemakingMessageParser : IMessageParser
{
    private readonly Regex reStart = new Regex("You start to work with the rope tool");
    private Regex reEnd = new Regex("You (almost made it, but the|create a) bow string");
    private Regex reSkill = new Regex("Ropemaking increased");

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
        return "Ropemaking";
    }
}

