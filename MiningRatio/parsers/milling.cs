using System;
using System.Text.RegularExpressions;

public class MillingMessageParser : IMessageParser
{
    private readonly Regex reStart = new Regex("You start to work with the grindstone on the");
    private readonly Regex reEnd = new Regex("You (almost made it, but the|create a) flour");
    private readonly Regex reSkill = new Regex("Milling increased");

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
        return "Milling";
    }
}

