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

public interface IMessageParser
{
    bool isActionStart(string message);
    bool isActionEnd(string message);
    bool isSkillGain(string message);
    string getName();
}

namespace MiningRatio
{
    public partial class MiningRatioForm : Form
    {
        Player player;
        LogWatcher? logWatcher = null;

        SkillTracker? skillTracker = null;

        public MiningRatioForm()
        {
            InitializeComponent();

            player = new Player();
            playerName.Text = player.PlayerName;
            wurmFolder.Text = player.WurmDir;

            loadMessageParsers();

            try {
                Start();
            } catch (Exception e) {
            	MessageBox.Show(e.Message);            
            }
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
            try
            {
                Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Start() 
        {
            UpdateDisplay();
            if (logWatcher != null)
            {
                logWatcher.Close();
            }

            skillTracker?.Reset();

            logWatcher = new LogWatcher();
            logWatcher.Add(player.LogDir, "_Event.*.txt");
            logWatcher.Add(player.LogDir, "_Skills.*.txt");
            logWatcher.PollInterval = 500;

            logWatcher.Notify += new LogWatcher.NotificationEventHandler(logWatcher_Notify);
        }

        delegate void UpdateDisplayCallback();
        private void UpdateDisplay()
        {
            if (lblActions.InvokeRequired)
            {
                UpdateDisplayCallback d = new UpdateDisplayCallback(UpdateDisplay);
                this.Invoke(d);
            }
            else if (skillTracker != null)
            {
                lblActions.Text = skillTracker.Actions.ToString();
                lblTicks.Text = skillTracker.Ticks.ToString();
                lblRatio.Text = skillTracker.GetRatio().ToString("P");
                lblSkillLast.Text = (skillTracker.LastActionRate * 3600).ToString("G5");
                lblSkillTicks.Text = (skillTracker.SkillRate * 3600).ToString("G5");
                lblSkillActions.Text = (skillTracker.TotalRate * 3600).ToString("G5");
                lblTotalSkill.Text = skillTracker.TotalSkill.ToString("G5");

                lblTotalTime.Text = skillTracker.TotalTime.ToString();
                lblSkillTime.Text = skillTracker.SkillTime.ToString();
            }
        }

        void logWatcher_Notify(object sender, string message)
        {
            StringReader reader = new StringReader(message);
            while (true) {
                String? line = reader.ReadLine();
                if (line == null)
                    break;
                skillTracker?.HandleLine(line);
            };
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

        private class CBWrapper 
        {
            private readonly IMessageParser mParser;
            private readonly string mName;

            public IMessageParser Parser
            {
                get { return mParser; }
            }


            public CBWrapper(IMessageParser parser) {
                mParser = parser;
                mName = parser.getName();
            }

            public CBWrapper(IMessageParser parser, string name) {
                mParser = parser;
                mName = name;
            }

            public override string ToString()
            {
                return mName;
            }
        }

        private IEnumerable<IMessageParser> loadPredefinedParsers()
        {
            yield return new MiningMessageParser();
            yield return new BlacksmithingMessageParser();
            yield return new CarpentryMessageParser();
            yield return new CoalmakingMessageParser();
            yield return new FineCarpentryMessageParser();
            yield return new GroomingMessageParser();
            yield return new MillingMessageParser();
            yield return new ProspectingMessageParser();
            yield return new RopemakingMessageParser();
            yield return new WeaponsmithingMessageParser();
            yield return new WoodcuttingMessageParser();
        }

        private void loadMessageParsers()
        {
            skillParser.Items.Clear();
            foreach (var parser in loadPredefinedParsers())
                skillParser.Items.Add(new CBWrapper(parser));
            skillParser.SelectedIndex = 0;

            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String dirname = path + "\\" + "WurmSkillRatio";

            if (!Directory.Exists(dirname))
                return;

            String[] files = Directory.GetFiles(dirname, "*.cs");
            var loader = new MessageParserLoader(dirname);
            foreach (String file in files)
            {
                try
                {
                    var handler = loader.LoadFile(file);
                    skillParser.Items.Add(new CBWrapper(handler, $"{handler.getName()} (Script)"));
                }
                catch (Exception e)
                {
                    AddLog(e.Message + "\n");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CBWrapper? wrapper = skillParser.SelectedItem as CBWrapper;
            if (wrapper != null && wrapper.Parser != null)
            {
                skillTracker = new(wrapper.Parser);
                skillTracker.UpdateDisplay += () => UpdateDisplay();
                skillTracker.AddLog += (text) => AddLog(text);
                Start();
            }
        }
    }

    public class MessageParserLoader {

        private readonly string? dirname;

        public MessageParserLoader() {
            dirname = null;
        }

        public MessageParserLoader(string dirname) {
            this.dirname = dirname;
        }

        public IMessageParser LoadFile(string file) {
            return LoadCode(File.ReadAllText(file));
        }

        public IMessageParser LoadCode(string code) {
            #if NETFRAMEWORK
            return CSScriptLibrary.CSScript.CodeDomEvaluator.LoadCode<IMessageParser>(code);
            #else
            return CSScriptLib.CSScript.RoslynEvaluator.LoadCode<IMessageParser>(code);
            #endif
        }
    }
}
