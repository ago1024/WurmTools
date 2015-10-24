using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using WurmUtils;

namespace WurmTimer
{

    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private List<Player> players;
        private LogWatcher logWatcher;

        public MainForm()
        {

            InitializeComponent();

            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "WurmTimer";
            trayIcon.Icon = Properties.Resources.wurm_icon;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);
            trayIcon.BalloonTipClicked += new EventHandler(trayIcon_BalloonTipClicked);

            this.Icon = Properties.Resources.wurm_icon;

            players = new List<Player>();
            if (!LoadConfig())
                players.Add(new Player());

            checkLog.Checked = true;
        }

        void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Show(); 
            Activate();
            BringToFront();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            if (value)
                ShowInTaskbar = value;
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

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        delegate void startTimerCallback(TimeSpan duration, String label);

        private void startTimer(TimeSpan duration, String label)
        {
            if (layoutPanel.InvokeRequired)
            {
                startTimerCallback d = new startTimerCallback(startTimer);
                this.Invoke(d, new object[] { duration, label });
            }
            else
            {
                CountDownTimer countDown = new CountDownTimer();

                layoutPanel.Controls.Add(countDown);
                countDown.Duration = duration;
                countDown.Label = label;
                countDown.Expired += new EventHandler(countDown_Expired);
                countDown.Start();
            }
        }

        void countDown_Expired(object sender, EventArgs e)
        {
            CountDownTimer countDown = sender as CountDownTimer;
            countDown.Click += new EventHandler(countDown_Click);

            trayIcon.ShowBalloonTip(5000, "Wurmtimer", String.Format("Countdown {0} expired", countDown.Label), ToolTipIcon.Info);
            FlashWindow();            
        }

        void countDown_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            control.Dispose();
        }

        private void btnShortMed_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(0, 30, 0), control.Text);
        }

        private void btnSleep_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(0, 5, 0), control.Text);
        }

        private void btnPrayer_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(0, 20, 0), control.Text);
        }

        private void btnLongMed_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(3, 0, 0), control.Text);
        }

        private void btnAlignment_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(0, 30, 0), control.Text);

        }

        private void btnSermon_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(3, 0, 0), control.Text);
        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            startTimer(new TimeSpan(dtCustom.Value.Hour, dtCustom.Value.Minute, dtCustom.Value.Second), control.Text);
        }


        private void layoutPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Width = layoutPanel.ClientSize.Width - 8;
            e.Control.ContextMenuStrip = timerContextMenu;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ContextMenuStrip context = item.Owner as ContextMenuStrip;
            CountDownTimer timer = context.SourceControl as CountDownTimer;
            timer.Stop();
            timer.Dispose();
        }

        private void layoutPanel_ClientSizeChanged(object sender, EventArgs e)
        {
            foreach (Control control in layoutPanel.Controls) {
                control.Width = layoutPanel.ClientSize.Width - 8;
            }
        }

        private bool LoadConfig()
        {

            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = path + "\\" + "WurmTimer.xml";

            if (!new FileInfo(filename).Exists)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator iter;
                iter = nav.Select("//WurmTimer/Player");
                while (iter.MoveNext())
                {
                    XmlElement element = iter.Current.UnderlyingObject as XmlElement;

                    Player player = new Player();
                    player.PlayerName = iter.Current.Evaluate("string(@name)") as String;
                    player.WurmDir = iter.Current.Evaluate("string(@wurmdir)") as String;
                    String testServer = iter.Current.Evaluate("string(@testserver)") as String;
                    player.TestServer = bool.TrueString.Equals(testServer, StringComparison.InvariantCultureIgnoreCase);

                    System.Diagnostics.Debug.WriteLine(String.Format("Configured player {0} in {1}", player.PlayerName, player.WurmDir));
                    this.players.Add(player);
                }               
            }
            catch (Exception e)
            {
                this.players.Clear();
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }

            return true;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public Int32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        [DllImport("user32.dll")]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public bool FlashWindow()
        {
            FLASHWINFO fw = new FLASHWINFO();

            fw.cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO)));
            fw.hwnd = this.Handle;
            fw.dwFlags = 0xf;
            fw.uCount = UInt32.MaxValue;

            return FlashWindowEx(ref fw);
        }

        private void checkLog_CheckedChanged(object sender, EventArgs e)
        {
            enableLogWatcher(checkLog.Checked);
        }

        private void enableLogWatcher(bool enable)
        {
            try
            {
                if (logWatcher != null)
                    logWatcher.Close();
                if (enable)
                {
                    logWatcher = new LogWatcher();

                    foreach (Player player in players)
                    {
                        logWatcher.Add(player.LogDir, "_Event.*.txt");
                        logWatcher.Add(player.LogDir, "_Skills.*.txt");
                        logWatcher.PollInterval = 5000;
                    }

                    logWatcher.FileNotify += new LogWatcher.FileNotificationEventHandler(logWatcher_FileNotify);
                }
                else
                {
                    logWatcher = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void logWatcher_FileNotify(object sender, string fullpath, string message)
        {
            String dirname = Path.GetDirectoryName(fullpath);
            Player player = null;

            foreach (Player p in players)
            {
                if (p.LogDir.Equals(dirname))
                {
                    player = p;
                    break;
                }
            }


            StringReader reader = new StringReader(message);
            String line;
            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    handleLine(player, line);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }            
        }

        private void handleLine(Player player, String line)
        {
            String prefix = "";
            if (player != null && players.Count > 1)
                prefix = player.PlayerName + ": ";

            if (Regex.IsMatch(line, "Alignment increased by"))
                startTimer(new TimeSpan(0, 30, 0), prefix + "Alignment gain");
            else if (Regex.IsMatch(line, "Faith increased by"))
                startTimer(new TimeSpan(0, 20, 0), prefix + "Faith");
            else if (Regex.IsMatch(line, "Lock picking increased by"))
                startTimer(new TimeSpan(0, 10, 0), prefix + "Lock picking");
            else if (Regex.IsMatch(line, "You finish your meditation"))
                startTimer(new TimeSpan(0, 28, 0), prefix + "Short meditation");
            else if (Regex.IsMatch(line, "You feel that it will take you a while before you are ready to meditate again"))
                startTimer(new TimeSpan(3, 0, 0), prefix + "Long meditation");
            else if (Regex.IsMatch(line, "You finish this sermon"))
                startTimer(new TimeSpan(3, 0, 0), prefix + "Sermon");
            else if (Regex.IsMatch(line, "You start using the sleep bonus\\."))
                startTimer(new TimeSpan(0, 5, 0), prefix + "Sleep bonus");
        }
    }
}
