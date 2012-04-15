using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using WurmUtils;

namespace MiningRatio
{
    public partial class MiningRatioForm : Form
    {
        Player player;
        LogWatcher logWatcher = null;
        Regex reMineSome = new Regex("You mine some ore|You mine some rock shards");
        Regex reStartMine = new Regex("You start to mine");
        Regex reMiningIncreased = new Regex("Mining increased");
        IFormatProvider numberParser = new CultureInfo("en-US").NumberFormat;


        int ticks;
        int actions;
        int actionStart;
        int actionEnd;
        double totalSkill;
        int totalTime;
        int skillTime;
        int actionDuration;
        double rel;
        double srel;
        double trel;

        List<String> deferred = new List<String>();

        public MiningRatioForm()
        {
            InitializeComponent();

            player = new Player();
            playerName.Text = player.PlayerName;
            wurmFolder.Text = player.WurmDir;

            Start();
        }

        private void playerName_TextChanged(object sender, EventArgs e)
        {
            player.PlayerName = playerName.Text;
        }

        private void wurmFolder_TextChanged(object sender, EventArgs e)
        {
            player.WurmDir = wurmFolder.Text;
        }

        private void startScan_Click(object sender, EventArgs e)        
        {
            Start();            
        }

        private void Start() 
        {
            ticks = 0;
            actions = 0;
            actionStart = -1;
            actionEnd = -1;
            totalSkill = 0;
            totalTime = 0;
            skillTime = 0;
            rel = 0;
            srel = 0;
            trel = 0;

            UpdateDisplay();
            if (logWatcher != null)
            {
                logWatcher.Close();
            }

            logWatcher = new LogWatcher();
            logWatcher.Add(player.LogDir, "_Event.*.txt");
            logWatcher.Add(player.LogDir, "_Skills.*.txt");
            logWatcher.PollInterval = 500;

            logWatcher.Notify += new LogWatcher.NotificationEventHandler(logWatcher_Notify);
        }

        private double getRatio()
        {
            if (actions > 0)
                return ticks / (1.0 * actions);
            else
                return 0.0;
        }


        delegate void UpdateDisplayCallback();
        private void UpdateDisplay()
        {
            if (lblActions.InvokeRequired)
            {
                UpdateDisplayCallback d = new UpdateDisplayCallback(UpdateDisplay);
                this.Invoke(d);
            }
            else
            {
                lblActions.Text = actions.ToString();
                lblTicks.Text = ticks.ToString();
                lblRatio.Text = getRatio().ToString("P");
                lblSkillLast.Text = (rel * 3600).ToString("G5");
                lblSkillTicks.Text = (srel * 3600).ToString("G5");
                lblSkillActions.Text = (trel * 3600).ToString("G5");
                lblTotalSkill.Text = totalSkill.ToString("G5");

                lblTotalTime.Text = new TimeSpan(0,0,totalTime).ToString();
                lblSkillTime.Text = new TimeSpan(0,0,skillTime).ToString();

            }
        }

        void logWatcher_Notify(object sender, string message)
        {
            StringReader reader = new StringReader(message);
            while (true) {
                String line = reader.ReadLine();
                if (line == null)
                    break;
                handleLine(line);
            };
        }

        private int GetMessageStamp(String message)
        {
            Match m = Regex.Match(message, @"\[(..):(..):(..)]");
            return Int32.Parse(m.Groups[1].Value) * 3600 +
                Int32.Parse(m.Groups[2].Value) * 60 +
                Int32.Parse(m.Groups[3].Value);
        }

        private void handleLine(String message)
        {
            try
            {
                if (reStartMine.IsMatch(message))
                {
                    actionStart = GetMessageStamp(message);
                }
                else if (reMineSome.IsMatch(message))
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

                    AddLog(String.Format("{0,-55}:{1,4}:{2,4}:{3,-7:G5}\n", message, ticks, actions, getRatio()));
                    UpdateDisplay();

                    foreach (String line in deferred) {
                        handleLine(line);
                    }
                    deferred.Clear();
                }
                else if (reMiningIncreased.IsMatch(message))
                {
                    int stamp = GetMessageStamp(message);
                    if (stamp < actionEnd)
                    {
                        deferred.Add(message);
                        AddLog(String.Format("Deferring {0}\n", message));
                    }
                    else
                    {
                        /* Get amount of skill gained */
                        Match m = Regex.Match(message, ".*increased by (.*) to.*");
                        double skill = Double.Parse(m.Groups[1].Value, numberParser);

                        /* Sum up total skill gained */
                        totalSkill += skill;
                        /* Sum up total time spent in skill gaining actions */
                        skillTime += actionDuration;

                        /* Skill gain per hour of the last action */
                        if (actionDuration == 0) { rel = 0.0; } else { rel = skill / actionDuration; }
                        /* Skill gain per hour of skill gaining actions */
                        if (skillTime == 0) { srel = 0.0; } else { srel = totalSkill / skillTime; }
                        /* Skill gain per hour of all mining actions */
                        if (totalTime == 0) { trel = 0.0; } else { trel = totalSkill / totalTime; }

                        /* Print ratio, number of actions, skill gain per hour of the last action, skill gaining actions and all actions */
                        ticks++;

                        AddLog(String.Format("{0,-55}:{1,4}:{2,4}:{3,-7:G5}:{4,-7:G5}:{5,-7:G5}:{6,-7:G5}\n",
                             message, ticks, actions, getRatio(),
                             rel * 3600, srel * 3600, trel * 3600));
                        UpdateDisplay();
                    }
                }
            }
            catch (Exception e)
            {
                AddLog(e.Message + "\n");
            }
        }

        delegate void AddLogCallback(string text);
        private void AddLog(String text)
        {
            if (logText.InvokeRequired)
            {
                AddLogCallback d = new AddLogCallback(AddLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                logText.AppendText(text);
            }
        }
    }
}
