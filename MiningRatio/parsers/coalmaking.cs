using System.Text.RegularExpressions;

public class CoalmakingMessageParser : IMessageParser
{
    private readonly Regex reStart = new Regex("You start to work with the (dirt|log) on the pile");
    private readonly Regex reEnd = new Regex("You attach the (dirt|log) to the pile|You almost made it, but the (dirt|log) is damaged");
    private readonly Regex reSkill = new Regex("Coal-making increased");

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
        return "Coalmaking";
    }
}

