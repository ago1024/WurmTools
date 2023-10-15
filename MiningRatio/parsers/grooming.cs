using System;
using System.Text.RegularExpressions;

public class GroomingMessageParser : IMessageParser
{
    private readonly Regex reStart = new Regex("You start to tend");
    private readonly Regex reEnd = new Regex("You have now tended|shys away and interrupts the action");

    private readonly Regex reSkill = new Regex("Animal husbandry increased");

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
        return "Grooming";
    }
}

