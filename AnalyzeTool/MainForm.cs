using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.InteropServices;
using WurmUtils;

namespace AnalyzeTool
{
    public partial class MainForm : Form, IMessageFilter
    {

        private String mapFileName;
        private String autoSaveFileName;
        private AnalyzeMap map;
        private Font font;
        private List<Player> players = new List<Player>();
        private MapTool activeTool = null;
        private Boolean renderAnalyzerOverlay = true;
        private Boolean remderQualityOverlay = true;
        private int textureWidth = 32;
        private int textureHeight = 32;
        private Tile popupTile;

        public Tile PopupTile
        {
            get { return popupTile; }
            set { popupTile = value; }
        }

        private abstract class MapTool
        {
            public abstract bool UseTool(AnalyzeMap map, Tile tile);
        }

        private class TileTypeTool : MapTool
        {
            private TileType tileType;

            public TileType TileType { get { return tileType; } }

            public TileTypeTool(TileType tileType)
            {
                this.tileType = tileType;
            }


            public override bool UseTool(AnalyzeMap map, Tile tile)
            {
                if (map[tile].Type != tileType)
                {
                    map[tile].Type = tileType;
                    return true;
                }
                return false;
            }
        }

        public void NewMap()
        {
            gridControl1.GridSizeX = 16;
            gridControl1.GridSizeY = 16;

            map = new AnalyzeMap(gridControl1.GridSizeX, gridControl1.GridSizeY);
            map.OnResize += new AnalyzeMap.MapResizeHandler(map_OnResize);
            mapFileName = null;
            autoSaveFileName = null;
        }

        public MainForm()
        {
            InitializeComponent();
            font = new Font("Verdana", 18);

            NewMap();

            LoadConfig();

            if (players.Count == 0)
                players.Add(new Player());

            foreach (Player player in players)
            {
                LogParser parser = new LogParser(player);
                parser.OnAnalyze += new LogParser.AnalyzeEventHandler(parser_OnAnalyze);
                parser.Start();
            }

            Application.AddMessageFilter(this);
            gridControl1.MouseWheel += new MouseEventHandler(gridControl1_MouseWheel);
        }

        void gridControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                Point hotSpot = new Point(e.Location.X, e.Location.Y);
                hotSpot.Offset(gridControl1.Location);
                GridControl.Cell cell = gridControl1.CellFromPoint(e.Location.X, e.Location.Y, false);
                Rectangle cellRect = gridControl1.RectFromCell(cell.X, cell.Y);
                Point hotSpotInCell = new Point(e.Location.X - cellRect.Left, e.Location.Y - cellRect.Top);

                if (e.Delta < 0)
                {
                    if (gridControl1.CellWidth > 4)
                    {
                        gridControl1.CellWidth /= 2;
                        gridControl1.CellHeight /= 2;
                        hotSpotInCell.X /= 2;
                        hotSpotInCell.Y /= 2;
                    }
                }
                else
                {
                    if (gridControl1.CellWidth < 256)
                    {
                        gridControl1.CellWidth *= 2;
                        gridControl1.CellHeight *= 2;
                        hotSpotInCell.X *= 2;
                        hotSpotInCell.Y *= 2;
                    }
                }
                if (gridControl1.CellWidth < 32)
                    gridControl1.BorderSize = 0;
                else
                    gridControl1.BorderSize = 1;

                this.textureWidth = gridControl1.CellWidth;
                this.textureHeight = gridControl1.CellHeight;
                this.textureCache.Clear();

                Rectangle newCellRect = gridControl1.RectFromCell(cell.X, cell.Y);
                Point newHotSpot = new Point(newCellRect.Left + hotSpotInCell.X, newCellRect.Top + hotSpotInCell.Y);
                Point newLocation = new Point(hotSpot.X - newHotSpot.X, hotSpot.Y - newHotSpot.Y);
                if (newLocation.X > 0)
                    newLocation.X = 0;
                if (newLocation.Y > 0)
                    newLocation.Y = 0;

                panel1.HorizontalScroll.Value = -newLocation.X;
                panel1.VerticalScroll.Value = -newLocation.Y;
                //gridControl1.Location = newLocation;
                gridControl1.Invalidate();

                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        void map_OnResize(object sender, int newX, int newY, int dx, int dy)
        {
            gridControl1.GridSizeX = map.SizeX;
            gridControl1.GridSizeY = map.SizeY;
            gridControl1.Redraw();
        }

        private void AnalyzeFile(String fileName)
        {
            try
            {
                LogParser parser = new LogParser();
                parser.OnAnalyze += new LogParser.AnalyzeEventHandler(parser_OnAnalyze);

                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    parser.ParseLine(line);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not analyze the file: " + ex.Message);
            }
        }

        private bool LoadConfig()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = path + "\\" + "AnalyzeTool.xml";

            if (!new FileInfo(filename).Exists)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XPathNavigator nav = doc.CreateNavigator();

                XPathNodeIterator iter;
                iter = nav.Select("//AnalyzeTool/Player");
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


        void parser_OnAnalyze(object sender, List<AnalyzeMatch> matches)
        {
            Debug.Print("### Analyze results");
            foreach (AnalyzeMatch match in matches)
            {
                Debug.Print("{0}  {1} {2}", match.Distance, match.Type, match.Quality);
            }

            Debug.Print("{0}", new AnalyzeResult(matches));

            AddResult(new AnalyzeResult(matches));
        }

        delegate void AddResultCallback(AnalyzeResult result);
        private void AddResult(AnalyzeResult result)
        {
            if (resultsBox.InvokeRequired)
            {
                AddResultCallback d = new AddResultCallback(AddResult);
                this.Invoke(d, new object[] { result });
            }
            else
            {
                resultsBox.Items.Add(result);
                resultsBox.SelectedItem = result;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DrawCellBackground(GridControl.Cell cell, Rectangle area, Graphics graphics)
        {
            Image texture = GetTileTexture(cell.X, cell.Y);
            if (texture == null)
            {
                switch (map[cell.X, cell.Y].Type)
                {
                    case TileType.Tunnel:
                        graphics.FillRectangle(Brushes.White, area);
                        break;
                    case TileType.Rock:
                        graphics.FillRectangle(Brushes.Gray, area);
                        break;
                    default:
                        graphics.FillRectangle(Brushes.Green, area);
                        break;
                }
            }
            else
            {
                TileStatus tile = map[cell.X, cell.Y];
                if (tile.Estimates == null && tile.Found == null)
                {
                    float b = 0.7f;
                    ColorMatrix cm = new ColorMatrix(new float[][]
                            {
                                new float[] {b, 0, 0, 0, 0},
                                new float[] {0, b, 0, 0, 0},
                                new float[] {0, 0, b, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {0, 0, 0, 0, 1},
                            });
                    ImageAttributes imgAttr = new ImageAttributes();
                    imgAttr.SetColorMatrix(cm);

                    Point[] points =
                    {
                        new Point(area.Left, area.Top),
                        new Point(area.Right, area.Top),
                        new Point(area.Left, area.Bottom),
                    };
                    Rectangle srcRect = new Rectangle(0, 0, area.Width, area.Height);

                    graphics.DrawImage(texture, points, srcRect, GraphicsUnit.Pixel, imgAttr);
                }
                else
                {
                    graphics.DrawImage(texture, area);
                }
            }
            texture = GetSaltTexture(CellToTile(cell));
            if (texture != null)
            {
                graphics.DrawImage(texture, area);
            }
            texture = GetFlintTexture(CellToTile(cell));
            if (texture != null)
            {
                graphics.DrawImage(texture, area);
            }
        }

        private void DrawCellQuality(GridControl.Cell cell, Rectangle area, Graphics graphics)
        {
            Image texture = GetTileQualityTexture(CellToTile(cell));
            if (texture != null)
            {
                graphics.DrawImage(texture, area);
            }
        }
       

        private void DrawCellAnalyzeLayer(GridControl.Cell cell, Rectangle area, Graphics graphics)
        {
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;
            if (map[cell.X, cell.Y].IsEmpty)
            {
            }
            else if (map[cell.X, cell.Y].IsSet)
            {
            }
            else if (map[cell.X, cell.Y].IsUndecided)
            {
                graphics.DrawString("?", font, Brushes.Red, area, drawFormat);
            }

            if (map[cell.X, cell.Y].Result != null)
            {
                graphics.DrawRectangle(Pens.Red, new Rectangle(area.Left, area.Top, area.Width - 1, area.Height - 1));
            }
        }

        private void gridControl1_OnCellPaint(object sender, GridControl.Cell cell, PaintEventArgs eventArgs)
        {
            DrawCellBackground(cell, eventArgs.ClipRectangle, eventArgs.Graphics);
            if (remderQualityOverlay)
                DrawCellQuality(cell, eventArgs.ClipRectangle, eventArgs.Graphics);
            if (renderAnalyzerOverlay)
                DrawCellAnalyzeLayer(cell, eventArgs.ClipRectangle, eventArgs.Graphics);
        }

        private void RepaintTile(Tile tile)
        {
            GridControl.Cell cell = TileToCell(tile);;
            gridControl1.PaintCell(cell.X, cell.Y);
        }

        private GridControl.Cell TileToCell(Tile tile)
        {
            return new GridControl.Cell(tile.X, tile.Y);
        }

        private Tile CellToTile(GridControl.Cell cell)
        {
            return new Tile(cell.X, cell.Y);
        }

        private void gridControl1_CellClick(object sender, GridControl.Cell cell, MouseEventArgs eventArgs)
        {          
            if (eventArgs.Button == MouseButtons.Left && (Control.ModifierKeys & Keys.Control) != 0)
            {
                map[cell.X, cell.Y].SetEmpty();
                Recalculate();
            }
            else if (eventArgs.Button == MouseButtons.Left)
            {
                if (activeTool != null)
                {
                    if (activeTool.UseTool(map, CellToTile(cell)))
                    {
                        gridControl1.PaintCell(cell.X, cell.Y);
                    }
                }
            }
            else if (eventArgs.Button == MouseButtons.Right)
            {
                PopupTile = CellToTile(cell);
                Rectangle rect = gridControl1.RectFromCell(cell.X, cell.Y);
                removeAnalyzeInfoToolStripMenuItem.Enabled = map[PopupTile].Result != null;
                gridContextMenu.Show(gridControl1, new Point(eventArgs.Location.X + rect.Left, eventArgs.Y + rect.Top));
            }
        }

        private void resultsBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (resultsBox.SelectedItem != null)
                resultsBox.DoDragDrop(resultsBox.SelectedItem, DragDropEffects.Move);
        }

        private void gridControl1_DragEnter(object sender, DragEventArgs e)
        {
            AnalyzeResult result = e.Data.GetData(typeof(AnalyzeResult)) as AnalyzeResult;
            if (result != null)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void gridControl1_DragOver(object sender, DragEventArgs e)
        {
        }

        private void gridControl1_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = gridControl1.PointToClient(new Point(e.X,e.Y));
            System.Diagnostics.Debug.Print("DragDrop {0},{1}", pt.X, pt.Y);
            AnalyzeResult result = e.Data.GetData(typeof(AnalyzeResult)) as AnalyzeResult;
            if (result != null)
            {
                GridControl.Cell cell = gridControl1.CellFromPoint(pt.X, pt.Y);
                if (cell != null)
                {
                    map.SetResult(cell.X, cell.Y, result);
                    gridControl1.Redraw();
                }
            }
        }

        private void Recalculate()
        {
            map.Refresh();
            gridControl1.Redraw();
        }

        private void gridControl1_CellMouseMove(object sender, GridControl.Cell cell, MouseEventArgs eventArgs)
        {
            if (eventArgs.Button == MouseButtons.Left && activeTool != null && Control.ModifierKeys == Keys.None)
            {
                if (activeTool.UseTool(map, CellToTile(cell)))
                {
                    gridControl1.PaintCell(cell.X, cell.Y);
                }
            }
        }

        private void gridControl1_CellMouseEnter(object sender, GridControl.Cell cell, MouseEventArgs eventArgs)
        {
            String tileType;
            if (map[cell.X, cell.Y].Quality != Quality.Unknown)
            {
                tileType = String.Format("{2},{3} {0} ({1})", map[cell.X, cell.Y].Type, map[cell.X, cell.Y].Quality, cell.X, cell.Y);
            } else {
                tileType = String.Format("{2},{3} {0}", map[cell.X, cell.Y].Type, map[cell.X, cell.Y].Quality, cell.X, cell.Y);
            }

            if (map[cell.X, cell.Y].IsSet || map[cell.X, cell.Y].IsUndecided || map[cell.X, cell.Y].HasSalt || map[cell.X, cell.Y].HasFlint)
            {
                toolStripStatusLabel1.Text = String.Format("{0}; {1}", tileType, map[cell.X, cell.Y].ToString());
            }
            else
            {
                toolStripStatusLabel1.Text = tileType;
            }
        }

        private void gridControl1_MouseLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void analyzeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = players[0].LogDir;
            dialog.Filter = "Event log (_Event*.txt)|_Event*.txt|Logfiles (*.txt)|*.txt|All files|*.*";
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AnalyzeFile(dialog.FileName);
            }
        }

        Dictionary<TileType, Bitmap> textureCache = new Dictionary<TileType, Bitmap>();
        private Bitmap GetTileTexture(int x, int y)
        {
            TileType type = map[x, y].Type;
            if (!textureCache.ContainsKey(type))
            {
                Bitmap bitmap = GetTileTexture(type);
                if (bitmap == null)
                {
                    textureCache.Add(type, null);
                } 
                else if (bitmap.Width != textureWidth || bitmap.Height != textureHeight)
                {
                    Bitmap scaled = new Bitmap(bitmap, new Size(textureWidth, textureHeight));
                    textureCache.Add(type, scaled);
                }
                else
                {
                    textureCache.Add(type, bitmap);
                }
            }
            return textureCache[type];
        }

        private Bitmap GetTileTexture(TileType type)
        {
            switch (type)
            {
                case TileType.Tunnel:
                    return AnalyzeTool.Properties.Resources.slab;
                case TileType.Nothing:
                case TileType.Unknown:
                case TileType.Rock:
                    return AnalyzeTool.Properties.Resources.rock;
                case TileType.Copper:
                    return AnalyzeTool.Properties.Resources.copperore;
                case TileType.Gold:
                    return AnalyzeTool.Properties.Resources.goldore;
                case TileType.Iron:
                    return AnalyzeTool.Properties.Resources.ironore;
                case TileType.Lead:
                    return AnalyzeTool.Properties.Resources.leadore;
                case TileType.Marble:
                    return AnalyzeTool.Properties.Resources.marbleshards;
                case TileType.Silver:
                    return AnalyzeTool.Properties.Resources.silverore;
                case TileType.Slate:
                    return AnalyzeTool.Properties.Resources.slatevein;
                case TileType.Tin:
                    return AnalyzeTool.Properties.Resources.tinore;
                case TileType.Zinc:
                    return AnalyzeTool.Properties.Resources.zincore;
                case TileType.Reinforced:
                    return AnalyzeTool.Properties.Resources.reinforcedcave;
                case TileType.RockSalt:
                    return AnalyzeTool.Properties.Resources.rocksalt;
                case TileType.SandStone:
                    return AnalyzeTool.Properties.Resources.sandstonevein;
                default:
                    return null;
            }
        }

        private Bitmap GetTileQualityTexture(Tile tile)
        {
            Quality quality = map[tile.X, tile.Y].Quality;
            switch (quality)
            {
                case Quality.Poor:
                case Quality.Acceptable:
                    return AnalyzeTool.Properties.Resources.qualitypoor;
                case Quality.Normal:
                    return AnalyzeTool.Properties.Resources.qualitynormal;
                case Quality.Good:
                    return AnalyzeTool.Properties.Resources.qualitygood;
                case Quality.VeryGood:
                    return AnalyzeTool.Properties.Resources.qualityverygood;
                case Quality.Utmost:
                    return AnalyzeTool.Properties.Resources.qualityutmost;
                default:
                    return null;
            }
        }

        private Bitmap GetSaltTexture(Tile tile)
        {
            if (map[tile.X, tile.Y].HasSalt)
            {
                return AnalyzeTool.Properties.Resources.salt;
            }
            else
            {
                return null;
            }
        }

        private Bitmap GetFlintTexture(Tile tile)
        {
            if (map[tile.X, tile.Y].HasFlint)
            {
                return AnalyzeTool.Properties.Resources.flint;
            }
            else
            {
                return null;
            }
        }

        private void setCheckedTool(object sender) 
        {
            if (sender == null || activeTool == null)
            {
                setCheckedToolButton(null);
                setCheckedToolMenuItem(null);
            }
            else if (sender is ToolStripButton)
            {
                setCheckedToolButton(sender as ToolStripButton);
            }
            else if (sender is ToolStripMenuItem)
            {
                setCheckedToolMenuItem(sender as ToolStripMenuItem);
            }
        }

        private void setCheckedToolMenuItem(ToolStripMenuItem item)
        {
            setCheckedToolButton(null);
            if (item != null)
                toolStripOre.Image = item.Image;
            else
                toolStripOre.Image = toolStripIron.Image;
            foreach (ToolStripItem dropDownItem in toolStripOre.DropDownItems)
            {
                if (dropDownItem is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuItem = dropDownItem as ToolStripMenuItem;
                    menuItem.Checked = menuItem == item;
                }
            }
        }

        private void setCheckedToolButton(ToolStripButton sender)
        {
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                ToolStripButton button = item as ToolStripButton;
                if (button != null)
                {
                    button.Checked = button == sender;
                }
            }
        }

        private void setActiveTool(TileType tileType)
        {
            TileTypeTool active = activeTool as TileTypeTool;
            if (active != null && active.TileType == tileType) {
                activeTool = null;
            } else {
                activeTool = new TileTypeTool(tileType);
            }
        }

        private void toolStripTunnel_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Tunnel);
            setCheckedTool(sender);
        }

        private void toolStripRock_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Rock);
            setCheckedTool(sender);
        }

        private void toolStripReinforced_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Reinforced);
            setCheckedTool(sender);
        }

        private void copperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Copper);
            setCheckedTool(sender);
        }

        private void toolStripLead_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Lead);
            setCheckedTool(sender);
        }

        private void toolStripGold_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Gold);
            setCheckedTool(sender);
        }

        private void toolStripIron_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Iron);
            setCheckedTool(sender);
        }

        private void toolStripMarble_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Marble);
            setCheckedTool(sender);
        }

        private void toolStripSilver_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Silver);
            setCheckedTool(sender);
        }

        private void toolStripSlate_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Slate);
            setCheckedTool(sender);
        }

        private void toolStripTip_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Tin);
            setCheckedTool(sender);
        }

        private void toolStripZinc_Click(object sender, EventArgs e)
        {
            setActiveTool(TileType.Zinc);
            setCheckedTool(sender);
        }

        private void resetAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMap();
            gridControl1.Redraw();
        }

        private void resetAnalyzedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.ClearResults();
            gridControl1.Redraw();
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizeMapForm dialog = new ResizeMapForm();
            dialog.WidthBox.Text = map.SizeX.ToString();
            dialog.HeightBox.Text = map.SizeY.ToString();
            dialog.LeftBox.Text = "0";
            dialog.TopBox.Text = "0";

            if (dialog.ShowDialog() == DialogResult.OK) 
            {
                try
                {
                    int newX = Int32.Parse(dialog.WidthBox.Text);
                    int newY = Int32.Parse(dialog.HeightBox.Text);
                    int dX = Int32.Parse(dialog.LeftBox.Text);
                    int dY = Int32.Parse(dialog.TopBox.Text);
                    map.ResizeMap(newX, newY, dX, dY);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error resizing the map: " + ex.Message);
                }
            }

        }

        private void SaveMap(String filename)
        {
            try
            {
                using (Stream stream = new FileStream(filename, FileMode.Create))
                {
                    map.Save(stream);
                    mapFileName = filename;
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving the map: " + ex.Message);
            }
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoSaveFileName != null)
            {
                SaveMap(autoSaveFileName);
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Analyze tool map (*.atm)|*.atm";
                dialog.FilterIndex = 1;
                dialog.FileName = mapFileName;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveMap(dialog.FileName);
                    autoSaveFileName = dialog.FileName;
                }
            }
        }

        private void exportBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG files (*.jpg,*.jpeg)|*.jpg *.jpeg|PNG files (*.png)|*.png";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmap = new Bitmap(gridControl1.ClientSize.Width, gridControl1.ClientSize.Height);
                Boolean oldState = renderAnalyzerOverlay;
                renderAnalyzerOverlay = false;
                gridControl1.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                renderAnalyzerOverlay = oldState;

                bitmap.Save(dialog.FileName);
            }
        }

        private void LoadMap(String filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open))
            {
                this.map = AnalyzeMap.Load(stream);
                gridControl1.GridSizeX = map.SizeX;
                gridControl1.GridSizeY = map.SizeY;
                map.OnResize += new AnalyzeMap.MapResizeHandler(map_OnResize);
                mapFileName = filename;
                autoSaveFileName = null;
                activeTool = null;
                setCheckedTool(null);
            }
        }

        private void openMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Analyze tool map (*.atm)|*.atm";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadMap(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the map: " + ex.Message);
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void SetTileType(Tile tile, TileType tileType)
        {
            if (map.IsValidTile(tile) && map[tile].Type != tileType)
            {
                map[tile].Type = tileType;
                RepaintTile(tile);
            }            
        }

        private void SetTileQuality(Tile tile, Quality quality)
        {
            if (map.IsValidTile(tile) && map[tile].Quality != quality)
            {
                map[tile].Quality = quality;
                RepaintTile(tile);
            }
        }

        private void rockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Rock);
        }

        private void tunnelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Tunnel);
        }

        private void reinforcedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Reinforced);
        }

        private void copperToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Copper);
        }

        private void goldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Gold);
        }

        private void ironToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Iron);
        }

        private void leadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Lead);
        }

        private void marbleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Marble);
        }

        private void silverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Silver);
        }

        private void slateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Slate);
        }

        private void tinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Tin);
        }

        private void zincToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.Zinc);
        }

        private void poorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.Poor);
        }

        private void acceptableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.Acceptable);
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.Normal);
        }

        private void goodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.Good);
        }

        private void veryGoodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.VeryGood);
        }

        private void utmostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileQuality(PopupTile, Quality.Utmost);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map.IsValidTile(PopupTile))
            {
                map[PopupTile].Type = TileType.Unknown;
                map[PopupTile].Quality = Quality.Unknown;
                map[PopupTile].Reset();
                RepaintTile(PopupTile);
            }
        }

        private void removeAnalyzeInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.ClearResult(PopupTile.X, PopupTile.Y);
            gridControl1.Redraw();
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QualityInputForm dialog = new QualityInputForm();
            if (map.IsValidTile(PopupTile) && dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int quality = Int32.Parse(dialog.InputBox.Text);
                    if (quality < 20 || quality > 100)
                        throw new Exception("Quality must be between 20 and 100");
                    map[PopupTile].ExactQuality = quality;
                    RepaintTile(PopupTile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to parse number: " + ex.Message);
                }
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }

        private int lastItemIndex = -1;
        private void resultsBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                int itemIndex = -1;
                if (resultsBox.ItemHeight != 0)
                {
                    itemIndex = e.Y / resultsBox.ItemHeight;
                    itemIndex += resultsBox.TopIndex;
                }
                if (itemIndex != lastItemIndex)
                {
                    lastItemIndex = itemIndex;
                    if (itemIndex >= 0 && itemIndex < resultsBox.Items.Count)
                    {
                        toolTip1.SetToolTip(resultsBox, resultsBox.Items[itemIndex].ToString());
                    }
                    else
                    {
                        toolTip1.Hide(resultsBox);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void rocksaltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.RockSalt);
        }

        private void sandstoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.SandStone);
        }

        private void rocksaltToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.RockSalt);
        }

        private void sandstoneToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetTileType(PopupTile, TileType.SandStone);
        }
    }

    public class MouseScrollPanel : Panel
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (this.VScroll && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                this.VScroll = false;
                base.OnMouseWheel(e);
                this.VScroll = true;
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }
    }
}
