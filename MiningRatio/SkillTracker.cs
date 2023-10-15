namespace MiningRatio;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

public class SkillTracker
{
    private readonly IMessageParser messageParser;
    private readonly NumberFormatInfo numberFormat;

    private int ticks;
    private int actions;
    private int actionStart;
    private int actionEnd;
    private double actionSkill;
    private double totalSkill;
    private int totalTime;
    private int skillTime;
    private int actionDuration;
    private double rel;
    private double srel;
    private double trel;

    public int Ticks { get => ticks; }
    public int Actions { get => actions; }
    public double LastActionSkill { get => actionSkill; }
    public double TotalSkill { get => totalSkill; }
    public TimeSpan TotalTime { get => TimeSpan.FromSeconds(totalTime); }
    public TimeSpan SkillTime { get => TimeSpan.FromSeconds(skillTime); }
    public TimeSpan LastActionDuration { get => TimeSpan.FromSeconds(actionDuration); }

    public double LastActionRate { get => rel; }
    public double SkillRate { get => srel; }
    public double TotalRate { get => trel; }

    private readonly List<string> deferred = new();

    public delegate void AddLogHandler(string text);
    public event AddLogHandler? AddLog;

    public delegate void UpdateDisplayHandler();
    public event UpdateDisplayHandler? UpdateDisplay;

    public SkillTracker(IMessageParser messageParser)
    {
        this.messageParser = messageParser;
        numberFormat = new NumberFormatInfo
        {
            CurrencyDecimalSeparator = "."
        };
    }

    public void Reset() {
        ticks = 0;
        actions = 0;
        actionStart = -1;
        actionEnd = -1;
        actionSkill = 0;
        totalSkill = 0;
        totalTime = 0;
        skillTime = 0;
        rel = 0;
        srel = 0;
        trel = 0;
    }

    public void ProcessLog(string log)
    {
        foreach(var line in log.Replace("\r\n", "\n").Split('\n'))
            HandleLine(line);
    }

    public void HandleLine(string message)
    {
        try
        {
            if (messageParser.isActionStart(message))
            {
                actionStart = GetMessageStamp(message);
            }
            else if (messageParser.isActionEnd(message))
            {
                actionEnd = GetMessageStamp(message);

                if (actionStart < 0)
                {
                    /* The script started during a mining action. The start timestamp is not valid. ignore. */
                }
                else
                {
                    /* Check for day wrap around */
                    while (actionStart > actionEnd)
                    {
                        actionStart -= 24 * 3600;
                    }
                    /* Calculate the duration of the mining action */
                    actionDuration = actionEnd - actionStart;
                    /* Sum up total time spent in mining actions */
                    totalTime += actionDuration;
                }
                actions++;

                AddLog?.Invoke(string.Format("{0,-55}:{1,4}:{2,4}:{3,-7:G5}\n", message, ticks, actions, GetRatio()));

                rel = 0.0;
                actionSkill = 0;
                if (deferred.Count > 0) {
                    /* Sum up total time spent in skill gaining actions */
                    skillTime += actionDuration;
                    foreach (string line in deferred)
                        HandleLine(line);
                    deferred.Clear();
                } else {
                    /* Skill gain per hour of all mining actions */
                    if (totalTime != 0) { trel = totalSkill / totalTime; }
                }
                UpdateDisplay?.Invoke();
            }
            else if (messageParser.isSkillGain(message))
            {
                int stamp = GetMessageStamp(message);
                if (stamp > actionEnd)
                {
                    deferred.Add(message);
                }
                else
                {
                    /* Get amount of skill gained */
                    Match m = Regex.Match(message, ".*increased by (.*) to.*");
                    double skill = double.Parse(m.Groups[1].Value.Replace(",", "."), numberFormat);

                    /* Sum up total skill gained */
                    totalSkill += skill;
                    actionSkill += skill;

                    /* Skill gain per hour of the last action */
                    if (actionDuration != 0) { rel += skill / actionDuration; }
                    /* Skill gain per hour of skill gaining actions */
                    if (skillTime != 0) { srel = totalSkill / skillTime; }
                    /* Skill gain per hour of all mining actions */
                    if (totalTime != 0) { trel = totalSkill / totalTime; }

                    /* Print ratio, number of actions, skill gain per hour of the last action, skill gaining actions and all actions */
                    ticks++;

                    AddLog?.Invoke(string.Format("{0,-55}:{1,4}:{2,4}:{3,-7:G5}:{4,-7:G5}:{5,-7:G5}:{6,-7:G5}\n", message, Ticks, Actions, GetRatio(), LastActionRate * 3600, SkillRate * 3600, TotalRate * 3600));
                }
            }
        }
        catch (Exception e)
        {
            AddLog?.Invoke(e.Message + "\n");
        }
    }

    public double GetRatio()
    {
        if (Actions > 0)
            return Ticks / (1.0 * Actions);
        else
            return 0.0;
    }



    private static int GetMessageStamp(string message)
    {
        Match m = Regex.Match(message, @"\[(..):(..):(..)]");
        return int.Parse(m.Groups[1].Value) * 3600 +
            int.Parse(m.Groups[2].Value) * 60 +
            int.Parse(m.Groups[3].Value);
    }


}