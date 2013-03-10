using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WurmUtils;

namespace ImproveTool
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
            trayIcon.Text = "ImproveTool";
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

            SetTool(Tools.Empty);
            enableLogWatcher(true);
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
        private bool LoadConfig()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = path + "\\" + "ImproveTool.xml";

            if (!new FileInfo(filename).Exists)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator iter;
                iter = nav.Select("//ImproveTool/Player");
                while (iter.MoveNext())
                {
                    XmlElement element = iter.Current.UnderlyingObject as XmlElement;

                    Player player = new Player();
                    player.PlayerName = iter.Current.Evaluate("string(@name)") as String;
                    player.WurmDir = iter.Current.Evaluate("string(@wurmdir)") as String;

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
                        logWatcher.PollInterval = 500;
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

        /*
        enum Tools
        {
            Pelt,
            Mallet,
            Log,
            Carvingknife,
            File,
            Hammer,
            Water,
            Lump,
            Whetstone
        }*/

        class Tools
        {
            public static Tools Pelt = new Tools(Properties.Resources.resource, 2, 6);
            public static Tools Log = new Tools(Properties.Resources.resource, 6, 6);
            public static Tools Water = new Tools(Properties.Resources.resource, 0, 3);
            public static Tools Rockshards = new Tools(Properties.Resources.resource, 10, 6);
            public static Tools Strings = new Tools(Properties.Resources.resource, 0, 7);
            public static Tools Leather = new Tools(Properties.Resources.resource, 2, 6);
            public static Tools Mallet = new Tools(Properties.Resources.tools, 1, 1);
            public static Tools Carvingknife = new Tools(Properties.Resources.tools, 0, 1);
            public static Tools File = new Tools(Properties.Resources.tools, 9, 1);
            public static Tools Hammer = new Tools(Properties.Resources.tools, 2, 1);
            public static Tools Lump = new Tools(Properties.Resources.tools, 13, 5);
            public static Tools Whetstone = new Tools(Properties.Resources.tools, 3, 4);
            public static Tools Chisel = new Tools(Properties.Resources.tools, 0, 1);
            public static Tools Needle = new Tools(Properties.Resources.tools, 8, 3);
            public static Tools Scissors = new Tools(Properties.Resources.tools, 8, 1);
            public static Tools Awl = new Tools(Properties.Resources.tools, 14, 1);
            public static Tools Leatherknife= new Tools(Properties.Resources.tools, 6, 2);

            public static Tools Empty = new Tools(Properties.Resources.tools, 10, 3);


            private Bitmap bitmap;
            public Bitmap Bitmap {
                get { return this.bitmap; }
            }

            public Tools(Bitmap source, int x, int y) {
                this.bitmap =  source.Clone(new Rectangle(x * 16, y * 16, 16, 16), source.PixelFormat);
            }
        }

        private void SetTool(Tools tool)
        {
            this.pictureBox1.Image = tool.Bitmap;
        }

        delegate void setLineCallback(String line);        
        private void SetLine(String line)
        {
            if (richTextBox1.InvokeRequired)
            {
                setLineCallback d = new setLineCallback(SetLine);
                this.Invoke(d, new object[] { line });
            }
            else
            {
                richTextBox1.Text = line;
                
            }
        }

        private struct Entry
        {
            public Regex pattern;
            public Tools tool;

            public Entry(String p, Tools t) {
                pattern = new Regex(p);
                tool = t;
            }
        }

        private Entry[] entries = 
        {                                      
            new Entry("You will want to polish", Tools.Pelt),
            new Entry("You must use a mallet"  , Tools.Mallet),
            new Entry("You must use a file", Tools.File),
            new Entry("You notice some notches", Tools.Carvingknife),
            new Entry("could be improved with (some more|a) log", Tools.Log),
            new Entry("has some irregularities that must be removed with a stone chisel", Tools.Chisel),
            new Entry("could be improved with (some more|a) rock shards", Tools.Rockshards),
            new Entry("could be improved with (some more|a) lump", Tools.Lump),
            new Entry("need to temper .* by dipping it in water while it's hot", Tools.Water),
            new Entry("has some dents that must be flattened by a hammer", Tools.Hammer),
            new Entry("needs to be sharpened with a whetstone", Tools.Whetstone),
            new Entry("It could be improved with (some more|a) string of cloth", Tools.Strings),
            new Entry("has some stains that must be washed away", Tools.Water),
            new Entry("A mallet must be used on the", Tools.Mallet),
            new Entry("has some excess leather that needs to be cut away with a leather knife", Tools.Leatherknife),
            new Entry("It could be improved with (some more|a) leather", Tools.Leather),
            new Entry("needs some holes punched with an awl", Tools.Awl),
        };


        private void handleLine(Player player, String line)
        {
            System.Diagnostics.Debug.WriteLine(line);

            foreach (Entry entry in entries)
            {
                if (entry.pattern.IsMatch(line))
                {
                    SetTool(entry.tool);
                    SetLine(line);
                    break;
                }
            }
        }

    }
}
