using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;

namespace WurmUtils
{
    class Days
    {
        public static string[] days = { "Ant", 
                          "Luck", 
                          "Wurm", 
                          "Wrath", 
                          "Tears", 
                          "Sleep", 
                          "Awakening" };
        public const int Ant = 0;
        public const int Luck = 1;
        public const int Wurm = 2;
        public const int Wrath = 3;
        public const int Tears = 4;
        public const int Sleep = 5;
        public const int Awakening = 6;

        public static int ByName(string day)
        {
            return Array.IndexOf(days, day);
        }

        public static string ByNumber(int day)
        {
            return days[day];
        }

        public int this[string day]
        {
            get { return ByName(day); }
        }

        public string this[int day]
        {
            get { return ByNumber(day); }
        }
    }

    class Starfalls
    {
        public static string[] starfalls = { "Diamonds", 
                               "Saw",
                               "Digging", 
                               "Leaf",
                               "Bears", 
                               "Snakes", 
                               "White Shark", 
                               "Fires", 
                               "Raven", 
                               "Dancers", 
                               "Omens", 
                               "Silence" };

        public const int Diamonds = 0;
        public const int Saw = 1;
        public const int Digging = 2;
        public const int Leaf = 3;
        public const int Bears = 4;
        public const int Snakes = 5;
        public const int WhiteShark = 6;
        public const int Fires = 7;
        public const int Raven = 8;
        public const int Dancers = 9;
        public const int Omens = 10;
        public const int Silence = 11;

        public static int ByName(string starfall)
        {
            return Array.IndexOf(starfalls, starfall);
        }

        public static string ByNumber(int starfall)
        {
            return starfalls[starfall];
        }

        public int this[string starfall]
        {
            get { return ByName(starfall); }
        }

        public string this[int starfall]
        {
            get { return ByNumber(starfall); }
        }
    }

    public struct WurmDateTime
    {
        private int starfall;
        private int day;
        private double time;

        public int Starfall
        {
            get { return starfall; }
            set { starfall = value; }
        }

        public int Week
        {
            get { return (day / 7) + 1; }
        }

        public int Day
        {
            get { return day; }
            set { day = value; }
        }

        public int DayOfWeek
        {
            get { return day % 7; }
        }

        public double Time
        {
            get { return time; }
            set { time = value; }
        }

        public String StarfallName
        {
            get { return Starfalls.ByNumber(starfall % 12); }
        }

        public String DayName
        {
            get { return Days.ByNumber(DayOfWeek); }
        }

        public String Text
        {
            get { return new TimeSpan(0, 0, (int)time) + " on " + DayName + " in week " + Week + " of the " + StarfallName + " starfall"; }
        }

        public void AddReal(TimeSpan span)
        {
            Add(new TimeSpan(0, 0, (int)span.TotalSeconds * 8));
        }

        public void Add(TimeSpan span)
        {
            time += span.TotalSeconds;
            while (time >= 86400)
            {
                day++;
                time -= 86400;
            }
            while (day >= 28)
            {
                starfall++;
                day -= 28;
            }
            while (starfall >= 12)
            {
                starfall -= 12;
            }
        }
    }

    public struct WurmDateTimeStamp
    {
        public DateTime real;
        public WurmDateTime wurm;

        public String toJSON()
        {
            TimeSpan span = new TimeSpan(0, 0, (int)wurm.Time);
            return String.Format(
                "{{\n"
                + "\t\"real\" : \"{0:yyyy-MM-ddTHH:mm:ssZ}\",\n"
                + "\t\"wurm\" : {{\n"
                + "\t\t\"starfall\" : {1},\n"
                + "\t\t\"day\" : {2},\n"
                + "\t\t\"hour\" : {3},\n"
                + "\t\t\"minute\" : {4},\n"
                + "\t\t\"second\" : {5},\n"
                + "\t\t\"time\" : {6},\n"
                + "\t\t\"text\" : \"{7}\"\n"
                + "\t}}\n"
                + "}}\n", real.ToUniversalTime(), wurm.Starfall, wurm.Day, 
                span.Hours, span.Minutes, span.Seconds, wurm.Time, wurm.Text);
        }
    }

    public class DateExtract
    {
        private static String RegistryKey = @"HKEY_CURRENT_USER\Software\JavaSoft\Prefs\com\wurmonline\client";

        public static String getLogPath()
        {
            Object wurmDir = Registry.GetValue(RegistryKey, "wurm_dir", null);
            Object wurmUser = Registry.GetValue(RegistryKey, "wurm_user", null);

            if (wurmDir == null || wurmUser == null)
                return null;

            String logsDir = wurmDir + "//players/" + wurmUser + "//logs";
            return logsDir.Replace("//", "\\");
        }

        public static bool parseDateTime(String date, ref WurmDateTime cur)
        {
            string[] days = { "day of the Ant", 
                              "Luck day", 
                              "day of the Wurm", 
                              "Wrath day", 
                              "day of Tears", 
                              "day of Sleep", 
                              "day of Awakening" };
            string[] starfalls = { "the starfall of Diamonds", 
                                   "the starfall of the Saw",
                                   "the starfall of the Digging", 
                                   "the starfall of the Leaf",
                                   "the Bear's starfall",
                                   "the Snake's starfall",
                                   "the White Shark starfall",
                                   "the starfall of Fires",
                                   "the Raven's starfall",
                                   "the starfall of Dancers", 
                                   "the starfall of Omens", 
                                   "the starfall of Silence" };
            Regex TimeEntry = new Regex(@"^It is (\d\d):(\d\d):(\d\d) on (.*) in week (\d) of (.*) in the year of (\d*).$");

            Match match = TimeEntry.Match(date);
            if (match.Success)
            {
                int day = Array.IndexOf(days, match.Groups[4].Value);
                int week = Int16.Parse(match.Groups[5].Value);
                int starfall = Array.IndexOf(starfalls, match.Groups[6].Value);
                int year = Int16.Parse(match.Groups[7].Value);

                int hour = Int16.Parse(match.Groups[1].Value);
                int minute = Int16.Parse(match.Groups[2].Value);
                int second = Int16.Parse(match.Groups[3].Value);

                if (day == -1 || week == -1 || starfall == -1 || year == -1 || hour == -1 || minute == -1 || second == -1)
                {
                    System.Diagnostics.Debug.WriteLine("Parse Error " + match.Groups[0] + String.Format(" {0} {1} {2} {3}", day, week, starfall, year));
                    return false;
                }

                cur.Day = day + 7 * week - 7;
                cur.Starfall = starfall;
                cur.Time = hour * 60 * 60 + minute * 60 + second;
                return true;
            }
            return false;
        }


        public static bool scanLogFile(String logFile, ref WurmDateTime cur, ref WurmDateTimeStamp stamp, DateTime maxDate)
        {

            bool foundMatch = false;

            System.Diagnostics.Debug.WriteLine(logFile);

            FileStream stream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(stream);

            Regex LoggingStarted = new Regex(@"^Logging started (\d{4}-\d{2}-\d{2})$");
            Regex LogEntry = new Regex(@"^\[(\d\d):(\d\d):(\d\d)] (.*)$");

            DateTime lastLogDate = new DateTime();
            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                Match match = LoggingStarted.Match(line);
                if (match.Success)
                {
                    lastLogDate = DateTime.Parse(match.Groups[1].Value);
                    continue;
                }
                match = LogEntry.Match(line);
                if (match.Success)
                {
                    DateTime currentLogDate = new DateTime(
                        lastLogDate.Year, lastLogDate.Month, lastLogDate.Day,
                        Int16.Parse(match.Groups[1].Value),
                        Int16.Parse(match.Groups[2].Value),
                        Int16.Parse(match.Groups[3].Value),
                        DateTimeKind.Local);
                    while (currentLogDate < lastLogDate)
                        currentLogDate = currentLogDate.AddDays(1); // Handle day wrap around
                    lastLogDate = currentLogDate;

                    if (currentLogDate > maxDate)
                    {
                        System.Diagnostics.Debug.WriteLine(currentLogDate + " > " + maxDate + ". Exiting");
                        break;
                    }

                    if (parseDateTime(match.Groups[4].Value, ref cur))
                    {
                        System.Diagnostics.Debug.WriteLine(lastLogDate + ": new Date " + match.Groups[4].Value);
                        stamp.wurm = cur;
                        stamp.real = lastLogDate;

                        foundMatch = true;
                    }
                }
            }

            reader.Close();
            stream.Close();

            return foundMatch;
        }
    }
}
