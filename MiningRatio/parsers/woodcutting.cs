using System.Text.RegularExpressions;

public class WoodcuttingMessageParser : IMessageParser
{
    private readonly Regex reStartKindling = new Regex("You start to work with the (hatchet|carving knife) on the (log|wood scraps)");
    private readonly Regex reEndKindling = new Regex("You (almost made it, but the|create a) kindling");

    private readonly Regex reStartCut  = new Regex("You start to cut down");
    private readonly Regex reEndCut  = new Regex("You chip away some wood|You cut down");

    private readonly Regex reStartChop  = new Regex("You start to chop up the felled tree");
    private readonly Regex reEndChop  = new Regex("You create a smaller log from the felled tree");

    private readonly Regex reSkill = new Regex("Woodcutting increased");

    public bool isActionStart(string message)
    {
        return reStartKindling.IsMatch(message) || reStartCut.IsMatch(message) || reStartChop.IsMatch(message);
    }
    public bool isActionEnd(string message)
    {
        return reEndKindling.IsMatch(message) || reEndCut.IsMatch(message) || reEndChop.IsMatch(message);
    }
    public bool isSkillGain(string message)
    {
        return reSkill.IsMatch(message);
    }
    public string getName()
    {
        return "Woodcutting";
    }
}

