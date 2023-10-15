using System.Text.RegularExpressions;

public class CarpentryMessageParser : IMessageParser
{

    private readonly Regex reStart = new Regex("You start (to improve|hammering|polishing|filing|carving) the");
    private readonly Regex reEnd4 = new Regex("You damage the .* a little|You ((will want|need) to polish|must use a file|must use a mallet|notice some notches)|The .* could be improved with (a|some more) log");
    private readonly Regex reEnd5 = new Regex("You improve the .* a bit");
    private readonly Regex reSkill = new Regex("Carpentry increased");

    private bool hasEnded = false;

    public bool isActionStart(string message)
    {
    if (reStart.IsMatch(message)) {
        hasEnded = false;
        return true;
    }
    return false;
    }
    public bool isActionEnd(string message)
    {
    if (hasEnded) {
        return false;
    }
    if (reEnd4.IsMatch(message) || reEnd5.IsMatch(message)) {
        hasEnded = true;
        return true;
    }
    return false;
    }
    public bool isSkillGain(string message)
    {
        return reSkill.IsMatch(message);
    }
    public string getName()
    {
        return "Carpentry";
    }
}

