using System;
using System.Text.RegularExpressions;

public class WeaponsmithingMessageParser : IMessageParser
{

    private readonly Regex reStartCarp = new Regex("You start (to improve|hammering|polishing|filing|carving) the");
    private readonly Regex reStartSmith = new Regex("You start (to improve|hammering|polishing|sharpening|tempering) the");
    private readonly Regex reEnd = new Regex("You damage the .* a little|You improve the .* a bit");
    private readonly Regex reEnd1 = new Regex("You need to temper .* by dipping it in water while it's hot");
    private readonly Regex reEnd2 = new Regex("You (will want|need) to polish .* with a pelt before you improve it");
    private readonly Regex reEnd3 = new Regex(".* (could be improved with a lump|has some dents that must be flattened by a hammer|needs to be sharpened with a whetstone)");
    private readonly Regex reEndCarp = new Regex("] You ((will want|need) to polish|must use a file|must use a mallet|notice some notches)|The .* could be improved with (a|some more) log");
    private readonly Regex reSkill = new Regex("Weapon smithing increased");

    private bool hasEnded = false;

    public bool isActionStart(string message)
    {
	if (reStartSmith.IsMatch(message) || reStartCarp.IsMatch(message)) {
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
	if (reEnd1.IsMatch(message) || reEnd2.IsMatch(message) || reEnd3.IsMatch(message) || reEnd.IsMatch(message) || reEndCarp.IsMatch(message)) {
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
        return "Weapon smithing";
    }
}

