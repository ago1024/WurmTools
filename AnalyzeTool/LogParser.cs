using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WurmUtils;

namespace AnalyzeTool
{

    public class AnalyzeMatch
    {
        private int distance;
        private String type;
        private String quality;

        public AnalyzeMatch(int distance, String type, String quality)
        {
            this.distance = distance;
            this.type = type;
            this.quality = quality;
        }

        public int Distance
        {
            get { return distance; }
        }

        public String Type
        {
            get { return type; }
        }

        public String Quality
        {
            get { return quality; }
        }
    }

    class LogParser
    {
        private Player player;
        private LogWatcher logWatcher;

        private Regex reStart = new Regex("You start to analyse");
        private Regex reStop = new Regex("You stop analysing");
        private Regex reEnd = new Regex("You finish analysing");
        private Regex reFail = new Regex("You cannot|You do not");
        private Regex reNextDistance = new Regex("You study it|You take a closer look");
        private List<AnalyzeMatch> matches = null;
        private List<Matcher> matchers = null;
        int distance = 0;

        public delegate void AnalyzeEventHandler(Object sender, List<AnalyzeMatch> matches);
        public event AnalyzeEventHandler OnAnalyze;

        private class Matcher
        {
            private Regex regex;
            private int distance;

            public Matcher(String regex, int distance)
            {
                this.regex = new Regex(regex);
                this.distance = distance;
            }

            public AnalyzeMatch Matches(String line)
            {
                Match match = this.regex.Match(line);
                if (match != null && match.Success)
                {
                    String quality = match.Groups["quality"].Value.Trim();
                    String type = match.Groups["type"].Value.Trim();
                    if (quality != null && quality.Length == 0)
                        quality = null;
                    return new AnalyzeMatch(distance, type, quality);
                }
                else
                {
                    return null;
                }
            }
        }

        public LogParser() : this(new Player())
        {
        }

        public LogParser(Player player)
        {
            this.player = player;
            this.InitializeMatchers();
        }

        private void InitializeMatchers()
        {
            this.matchers = new List<Matcher>();
            this.matchers.Add(new Matcher(@"vague trace of ((?<quality>.*) quality )?(?<type>.*)\.", 5));
            this.matchers.Add(new Matcher(@"minuscule trace of ((?<quality>.*) quality )?(?<type>.*)\.", 4));
            this.matchers.Add(new Matcher(@"faint trace of ((?<quality>.*) quality )?(?<type>.*)\.", 3));
            this.matchers.Add(new Matcher(@"slight trace of ((?<quality>.*) quality )?(?<type>.*)\.", 2));
            this.matchers.Add(new Matcher(@"trace of ((?<quality>.*) quality )?(?<type>.*)\.", 1));
        }

        public void Start()
        {
            if (logWatcher != null)
            {
                logWatcher.Close();
            }

            logWatcher = new LogWatcher();
            logWatcher.Add(player.LogDir, "_Event.*.txt");
            logWatcher.PollInterval = 500;

            logWatcher.Notify += new LogWatcher.NotificationEventHandler(logWatcher_Notify);
        }

        private void logWatcher_Notify(object sender, string message)
        {
            foreach (String line in message.Split('\n'))
                ParseLine(line);
        }

        public void ParseLine(String line)
        {
            if (this.reStart.IsMatch(line))
            {
                matches = new List<AnalyzeMatch>();
                distance = 1;
            }
            else if (this.reStop.IsMatch(line))
            {
                matches = null;
            }
            else if (this.reEnd.IsMatch(line))
            {
                if (OnAnalyze != null)
                {
                    OnAnalyze(this, matches);
                }
                matches = null;
            }
            else if (matches == null)
            {
            }
            else if (this.reNextDistance.IsMatch(line))
            {
                distance += 1;
            }
            else if (this.reFail.IsMatch(line))
            {
                matches.Add(new AnalyzeMatch(distance, null, null));
            }
            else
            {
                foreach (Matcher matcher in matchers)
                {
                    AnalyzeMatch match = matcher.Matches(line);
                    if (match != null)
                    {
                        matches.Add(match);
                        return;
                    }
                }
                System.Diagnostics.Debug.Print("Unmatched line {0}", line);
            }
        }
    }
}
