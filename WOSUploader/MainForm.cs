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
using WurmUtils;

namespace WOSUploader
{
    public partial class MainForm : Form
    {
        delegate void AddLogCallback(string text);
        private NotifyIcon trayIcon;
        private List<WOSPlayer> players;
        private List<FileSystemWatcher> watchers;

        public MainForm()
        {
            InitializeComponent();

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "WOSUploader";
            trayIcon.Icon = Properties.Resources.wurm_icon;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenuStrip = trayMenuStrip;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);

            this.Icon = Properties.Resources.wurm_icon;

            players = new List<WOSPlayer>();

            if (!LoadConfig())
                players.Add(new WOSPlayer());

            watchers = new List<FileSystemWatcher>();
            foreach (Player player in players)
            {
                SkillsWatcher watcher = new SkillsWatcher(player);
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                watcher.Error += new ErrorEventHandler(watcher_Error);

                watcher.EnableRaisingEvents = true;
                watchers.Add(watcher);
            }
        }

        void watcher_Error(object sender, ErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ToString());
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

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("File changed: " + e.FullPath);
            trayIcon.ShowBalloonTip(2000, "New skill dump", String.Format("Uploading skill dump {0}", e.Name), ToolTipIcon.Info);
            try
            {
                String username = Username.Text;
                String password = Password.Text;
                WOSPlayer player = (sender as SkillsWatcher).Player as WOSPlayer;
                if (player != null && player.WOSUser != null && player.WOSUser.Length > 0)
                    username = player.WOSUser;
                if (player != null && player.WOSPassword != null && player.WOSPassword.Length > 0)
                    password = player.WOSPassword;
                
                Uploader uploader = new Uploader(username, password);
                if (uploader.Login())
                {
                    uploader.Upload(e.FullPath);
                    trayIcon.ShowBalloonTip(2000, "New skill dump", String.Format("Uploaded skill dump {0}", e.Name), ToolTipIcon.Info);
                    AddLog(String.Format("Uploaded skill dump {0}\n", e.Name));
                }
                else
                {
                    trayIcon.ShowBalloonTip(2000, "Login failed", "Invalid username or password", ToolTipIcon.Error);
                    AddLog("Login failed. Invalid username or password\n");
                }
            }
            catch (Exception ex)
            {
                trayIcon.ShowBalloonTip(5000, ex.Message, ex.ToString(), ToolTipIcon.Error);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                AddLog(ex.ToString() + "\n");
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

        private void OnExit(object sender, EventArgs e)
        {
            foreach (FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
            }
            Application.Exit();
        }

        private void MainForm_DragOver(object sender, DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop))
                dea.Effect = DragDropEffects.Move;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (WOSPlayer player in players)
                {
                    List<String> files = new List<String>();
                    foreach (String filename in (string[])dea.Data.GetData(DataFormats.FileDrop))
                        if (filename.StartsWith(player.PlayerDir))
                            files.Add(filename);

                    if (files.Count == 0)
                        continue;

                    String username = Username.Text;
                    String password = Password.Text;
                    if (player != null && player.WOSUser != null && player.WOSUser.Length > 0)
                        username = player.WOSUser;
                    if (player != null && player.WOSPassword != null && player.WOSPassword.Length > 0)
                        password = player.WOSPassword;

                    Uploader uploader = new Uploader(username, password);
                    if (uploader.Login())
                    {
                        foreach (String filename in files)
                        {
                            uploader.Upload(filename);
                            trayIcon.ShowBalloonTip(2000, "New skill dump", String.Format("Uploaded skill dump {0}", filename), ToolTipIcon.Info);
                            AddLog(String.Format("Uploaded skill dump {0}\n", filename));
                        }
                    }
                    else
                    {
                        trayIcon.ShowBalloonTip(2000, "Login failed", "Invalid username or password", ToolTipIcon.Error);
                        AddLog("Login failed. Invalid username or password\n");
                    }
                }
            }
        }

        private bool LoadConfig()
        {

            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = path + "\\" + "WOSUploader.xml";

            if (!new FileInfo(filename).Exists)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator iter;
                iter = nav.Select("//WOSUploader/Player");
                while (iter.MoveNext())
                {
                    XmlElement element = iter.Current.UnderlyingObject as XmlElement;

                    WOSPlayer player = new WOSPlayer();
                    player.PlayerName = iter.Current.Evaluate("string(@name)") as String;
                    player.WurmDir = iter.Current.Evaluate("string(@wurmdir)") as String;
                    player.WOSUser = iter.Current.Evaluate("string(WurmOnlineSkillCompare/@username)") as String;
                    player.WOSPassword = iter.Current.Evaluate("string(WurmOnlineSkillCompare/@password)") as String;
                    if (player.WOSUser == null || player.WOSUser.Length == 0)
                        player.WOSUser = player.PlayerName;

                    AddLog(String.Format("Configured player {0} in {1}, using WOS user {2}\n", player.PlayerName, player.WurmDir, player.WOSUser));
                    this.players.Add(player);
                }
            }
            catch (Exception e)
            {
                this.players.Clear();
                AddLog(e.ToString() + "\n");
                return false;
            }

            if (players.Count == 1)
            {
                Username.Text = players[0].WOSUser;
                Password.Text = players[0].WOSPassword;
            }
            else if (players.Count > 1)
            {
                Username.Enabled = false;
            }

            return true;
        }
    }
}
