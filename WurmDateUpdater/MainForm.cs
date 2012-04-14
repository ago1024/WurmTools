using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using CSScriptLibrary;
using WurmUtils;

namespace WurmDateUpdater
{
    public interface IWurmDateHandler
    {
        void HandleWurmTimeStamp(WurmDateTimeStamp stamp);
    }

    public partial class MainForm : Form
    {
        delegate void AddLogCallback(string text);
        private NotifyIcon trayIcon;
        private List<Player> players;
        private Timer timer;
        private Dictionary<IWurmDateHandler, string> scripts;

        public MainForm()
        {
            InitializeComponent();

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "WurmDateUpdater";
            trayIcon.Icon = Properties.Resources.wurm_icon;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenuStrip = trayMenuStrip;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);

            this.Icon = Properties.Resources.wurm_icon;

            players = new List<Player>();
            scripts = new Dictionary<IWurmDateHandler, string>();

            if (!LoadConfig())
                players.Add(new Player());

            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1;
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            bool hasMatch = false;
            WurmDateTimeStamp stamp = new WurmDateTimeStamp();

            foreach (Player player in players)
            {
                String[] logFiles = Directory.GetFiles(player.LogDir, "_Event*.txt");
                Array.Sort(logFiles);
                Array.Reverse(logFiles);
                Array.Resize(ref logFiles, 1);

                WurmDateTime logTime = new WurmDateTime();
                WurmDateTimeStamp logStamp = new WurmDateTimeStamp();

                foreach (String logFile in logFiles)
                {
                    System.Diagnostics.Debug.WriteLine("Scanning file {0}", logFile);

                    if (DateExtract.scanLogFile(logFile, ref logTime, ref logStamp, DateTime.Now))
                    {
                        if (!hasMatch) {
                            hasMatch = true;
                            stamp = logStamp;
                        } else if (logStamp.real > stamp.real) {
                            stamp = logStamp;
                        }
                    }
                }

            }

            if (hasMatch)
            {
                AddLog(String.Format("Reference time: {0}: {1}\r\n", stamp.real.ToString(), stamp.wurm.Text));
                handleTimeStamp(stamp);
            }

            timer.Interval = 15 * 60 * 1000;
            timer.Enabled = true;
        }

        private void handleTimeStamp(WurmDateTimeStamp stamp)
        {
            foreach (IWurmDateHandler script in scripts.Keys)
            {
                try
                {
                    string scriptFile = scripts[script];
                    if (scriptFile != null)
                    {
                        Directory.SetCurrentDirectory(Path.GetDirectoryName(scriptFile));
                    }
                    script.HandleWurmTimeStamp(stamp);
                }
                catch (Exception e)
                {
                    AddLog(e.ToString());
                }
            }
        }

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

        void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (Visible)
            {
                Hide();
                ShowInTaskbar = false;
            }
            else
            {
                Show();
                Activate();
                BringToFront();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            trayIcon.Visible = false;
            Application.Exit();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }

        private bool LoadConfig()
        {

            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = path + "\\" + "WurmDateUpdater.xml";

            if (!new FileInfo(filename).Exists)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator iter;
                iter = nav.Select("//WurmDateUpdater/Player");
                while (iter.MoveNext())
                {
                    XmlElement element = iter.Current.UnderlyingObject as XmlElement;

                    Player player = new Player();
                    player.PlayerName = iter.Current.Evaluate("string(@name)") as String;
                    player.WurmDir = iter.Current.Evaluate("string(@wurmdir)") as String;

                    AddLog(String.Format("Configured player {0} in {1}\r\n", player.PlayerName, player.WurmDir));
                    this.players.Add(player);
                }

                iter = nav.Select("//WurmDateUpdater/Script");
                while (iter.MoveNext())
                {
                    try
                    {
                        String scriptFile = iter.Current.Evaluate("string(@script)") as String;
                        String script = iter.Current.Evaluate("string(.)") as String;

                        if (scriptFile != null && scriptFile.Length > 0)
                        {
                            if (File.Exists(scriptFile))
                            {
                                scriptFile = Path.GetFullPath(scriptFile);
                                CSScript.GlobalSettings.AddSearchDir(Path.GetDirectoryName(scriptFile));
                                AsmHelper asmHelper = new AsmHelper(CSScript.Load(scriptFile));
                                asmHelper.ProbingDirs = CSScript.GlobalSettings.SearchDirs.Split(';');

                                IWurmDateHandler handler = asmHelper.CreateObject("*").AlignToInterface<IWurmDateHandler>(true);
                                scripts.Add(handler, scriptFile);
                            }
                            else
                            {
                                throw new FileNotFoundException("The script file was not found", scriptFile);
                            }
                        }
                        else
                        {
                            Assembly csscript = CSScript.LoadCode(script);
                            IWurmDateHandler handler = csscript.CreateObject("*").AlignToInterface<IWurmDateHandler>(true);
                            scripts.Add(handler, null);
                        }
                    }
                    catch (Exception e)
                    {
                        AddLog(e.ToString());
                    }                    
                      
                }
            }
            catch (Exception e)
            {
                this.players.Clear();
                AddLog(e.ToString() + "\r\n");
                return false;
            }

            return true;
        }
    }
}
