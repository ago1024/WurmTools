using System;
using System.Text.RegularExpressions;

public class FineCarpentryMessageParser : IMessageParser
{

    private readonly Regex reStartCarp = new Regex("] You start (to improve|hammering|polishing|filing|carving) the");
    private readonly Regex reEnd = new Regex("You damage the .* a little|You improve the .* a bit");
    private readonly Regex reEndCarp = new Regex("] You (will want to polish|must use a file|must use a mallet|notice some notches)|The .* could be improved with (a|some more) log");
    private readonly Regex reSkill = new Regex("Fine carpentry increased");

    private bool hasEnded = false;

    public bool isActionStart(String message)
    {
	if (reStartCarp.IsMatch(message)) {
	    hasEnded = false;
	    return true;
	}
	return false;
    }
    public bool isActionEnd(String message)
    {
	if (hasEnded) {
	    return false;
	}
	if (reEnd.IsMatch(message) || reEndCarp.IsMatch(message)) {
	    hasEnded = true;
	    return true;
	}
	return false;
    }
    public bool isSkillGain(String message)
    {
    	return reSkill.IsMatch(message);
    }
    public String getName()
    {
        return "Fine carpentry";
    }
}

