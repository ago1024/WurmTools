namespace AnalyzeTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.resultsBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gridControl1 = new AnalyzeTool.GridControl();
            this.gridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tunnelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reinforcedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ironToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.marbleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.silverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zincToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.poorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acceptableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.veryGoodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAnalyzedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripRock = new System.Windows.Forms.ToolStripButton();
            this.toolStripTunnel = new System.Windows.Forms.ToolStripButton();
            this.toolStripReinforced = new System.Windows.Forms.ToolStripButton();
            this.toolStripOre = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripCopper = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGold = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripIron = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLead = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMarble = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSilver = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSlate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripZinc = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.gridContextMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsBox
            // 
            this.resultsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsBox.FormattingEnabled = true;
            this.resultsBox.Location = new System.Drawing.Point(663, 52);
            this.resultsBox.Name = "resultsBox";
            this.resultsBox.Size = new System.Drawing.Size(159, 589);
            this.resultsBox.TabIndex = 0;
            this.resultsBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resultsBox_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.gridControl1);
            this.panel1.Location = new System.Drawing.Point(12, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(645, 589);
            this.panel1.TabIndex = 1;
            // 
            // gridControl1
            // 
            this.gridControl1.AllowDrop = true;
            this.gridControl1.BorderSize = 1;
            this.gridControl1.CellHeight = 32;
            this.gridControl1.CellWidth = 32;
            this.gridControl1.GridSizeX = 16;
            this.gridControl1.GridSizeY = 16;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(529, 529);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.Text = "gridControl1";
            this.gridControl1.CellPaint += new AnalyzeTool.GridControl.OnCellPaintHandler(this.gridControl1_OnCellPaint);
            this.gridControl1.CellMouseClick += new AnalyzeTool.GridControl.OnCellMouseClickHandler(this.gridControl1_CellClick);
            this.gridControl1.CellMouseMove += new AnalyzeTool.GridControl.OnCellMouseMoveHandler(this.gridControl1_CellMouseMove);
            this.gridControl1.CellMouseEnter += new AnalyzeTool.GridControl.OnCellMouseEnterHandler(this.gridControl1_CellMouseEnter);
            this.gridControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragDrop);
            this.gridControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragEnter);
            this.gridControl1.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragOver);
            this.gridControl1.MouseLeave += new System.EventHandler(this.gridControl1_MouseLeave);
            // 
            // gridContextMenu
            // 
            this.gridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rockToolStripMenuItem,
            this.tunnelToolStripMenuItem,
            this.reinforcedToolStripMenuItem,
            this.oreToolStripMenuItem,
            this.qualityToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.gridContextMenu.Name = "gridContextMenu";
            this.gridContextMenu.Size = new System.Drawing.Size(153, 158);
            // 
            // rockToolStripMenuItem
            // 
            this.rockToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.rock;
            this.rockToolStripMenuItem.Name = "rockToolStripMenuItem";
            this.rockToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rockToolStripMenuItem.Text = "Rock";
            this.rockToolStripMenuItem.Click += new System.EventHandler(this.rockToolStripMenuItem_Click);
            // 
            // tunnelToolStripMenuItem
            // 
            this.tunnelToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.slab;
            this.tunnelToolStripMenuItem.Name = "tunnelToolStripMenuItem";
            this.tunnelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tunnelToolStripMenuItem.Text = "Tunnel";
            this.tunnelToolStripMenuItem.Click += new System.EventHandler(this.tunnelToolStripMenuItem_Click);
            // 
            // reinforcedToolStripMenuItem
            // 
            this.reinforcedToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.reinforcedcave;
            this.reinforcedToolStripMenuItem.Name = "reinforcedToolStripMenuItem";
            this.reinforcedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reinforcedToolStripMenuItem.Text = "Reinforced";
            this.reinforcedToolStripMenuItem.Click += new System.EventHandler(this.reinforcedToolStripMenuItem_Click);
            // 
            // oreToolStripMenuItem
            // 
            this.oreToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copperToolStripMenuItem,
            this.goldToolStripMenuItem,
            this.ironToolStripMenuItem,
            this.leadToolStripMenuItem,
            this.marbleToolStripMenuItem,
            this.silverToolStripMenuItem,
            this.slateToolStripMenuItem,
            this.tinToolStripMenuItem,
            this.zincToolStripMenuItem});
            this.oreToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.goldore;
            this.oreToolStripMenuItem.Name = "oreToolStripMenuItem";
            this.oreToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.oreToolStripMenuItem.Text = "Ore";
            // 
            // copperToolStripMenuItem
            // 
            this.copperToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.copperore;
            this.copperToolStripMenuItem.Name = "copperToolStripMenuItem";
            this.copperToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.copperToolStripMenuItem.Text = "Copper";
            this.copperToolStripMenuItem.Click += new System.EventHandler(this.copperToolStripMenuItem_Click_1);
            // 
            // goldToolStripMenuItem
            // 
            this.goldToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.goldore;
            this.goldToolStripMenuItem.Name = "goldToolStripMenuItem";
            this.goldToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.goldToolStripMenuItem.Text = "Gold";
            this.goldToolStripMenuItem.Click += new System.EventHandler(this.goldToolStripMenuItem_Click);
            // 
            // ironToolStripMenuItem
            // 
            this.ironToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.ironore;
            this.ironToolStripMenuItem.Name = "ironToolStripMenuItem";
            this.ironToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.ironToolStripMenuItem.Text = "Iron";
            this.ironToolStripMenuItem.Click += new System.EventHandler(this.ironToolStripMenuItem_Click);
            // 
            // leadToolStripMenuItem
            // 
            this.leadToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.leadore;
            this.leadToolStripMenuItem.Name = "leadToolStripMenuItem";
            this.leadToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.leadToolStripMenuItem.Text = "Lead";
            this.leadToolStripMenuItem.Click += new System.EventHandler(this.leadToolStripMenuItem_Click);
            // 
            // marbleToolStripMenuItem
            // 
            this.marbleToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.marbleshards;
            this.marbleToolStripMenuItem.Name = "marbleToolStripMenuItem";
            this.marbleToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.marbleToolStripMenuItem.Text = "Marble";
            this.marbleToolStripMenuItem.Click += new System.EventHandler(this.marbleToolStripMenuItem_Click);
            // 
            // silverToolStripMenuItem
            // 
            this.silverToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.silverore;
            this.silverToolStripMenuItem.Name = "silverToolStripMenuItem";
            this.silverToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.silverToolStripMenuItem.Text = "Silver";
            this.silverToolStripMenuItem.Click += new System.EventHandler(this.silverToolStripMenuItem_Click);
            // 
            // slateToolStripMenuItem
            // 
            this.slateToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.slateshards;
            this.slateToolStripMenuItem.Name = "slateToolStripMenuItem";
            this.slateToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.slateToolStripMenuItem.Text = "Slate";
            this.slateToolStripMenuItem.Click += new System.EventHandler(this.slateToolStripMenuItem_Click);
            // 
            // tinToolStripMenuItem
            // 
            this.tinToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.tinore;
            this.tinToolStripMenuItem.Name = "tinToolStripMenuItem";
            this.tinToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.tinToolStripMenuItem.Text = "Tin";
            this.tinToolStripMenuItem.Click += new System.EventHandler(this.tinToolStripMenuItem_Click);
            // 
            // zincToolStripMenuItem
            // 
            this.zincToolStripMenuItem.Image = global::AnalyzeTool.Properties.Resources.zincore;
            this.zincToolStripMenuItem.Name = "zincToolStripMenuItem";
            this.zincToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.zincToolStripMenuItem.Text = "Zinc";
            this.zincToolStripMenuItem.Click += new System.EventHandler(this.zincToolStripMenuItem_Click);
            // 
            // qualityToolStripMenuItem
            // 
            this.qualityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.poorToolStripMenuItem,
            this.acceptableToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.goodToolStripMenuItem,
            this.veryGoodToolStripMenuItem,
            this.utmostToolStripMenuItem,
            this.setToolStripMenuItem});
            this.qualityToolStripMenuItem.Name = "qualityToolStripMenuItem";
            this.qualityToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.qualityToolStripMenuItem.Text = "Quality";
            // 
            // poorToolStripMenuItem
            // 
            this.poorToolStripMenuItem.Name = "poorToolStripMenuItem";
            this.poorToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.poorToolStripMenuItem.Text = "Poor";
            this.poorToolStripMenuItem.Click += new System.EventHandler(this.poorToolStripMenuItem_Click);
            // 
            // acceptableToolStripMenuItem
            // 
            this.acceptableToolStripMenuItem.Name = "acceptableToolStripMenuItem";
            this.acceptableToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.acceptableToolStripMenuItem.Text = "Acceptable";
            this.acceptableToolStripMenuItem.Click += new System.EventHandler(this.acceptableToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // goodToolStripMenuItem
            // 
            this.goodToolStripMenuItem.Name = "goodToolStripMenuItem";
            this.goodToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.goodToolStripMenuItem.Text = "Good";
            this.goodToolStripMenuItem.Click += new System.EventHandler(this.goodToolStripMenuItem_Click);
            // 
            // veryGoodToolStripMenuItem
            // 
            this.veryGoodToolStripMenuItem.Name = "veryGoodToolStripMenuItem";
            this.veryGoodToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.veryGoodToolStripMenuItem.Text = "Very Good";
            this.veryGoodToolStripMenuItem.Click += new System.EventHandler(this.veryGoodToolStripMenuItem_Click);
            // 
            // utmostToolStripMenuItem
            // 
            this.utmostToolStripMenuItem.Name = "utmostToolStripMenuItem";
            this.utmostToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.utmostToolStripMenuItem.Text = "Utmost";
            this.utmostToolStripMenuItem.Click += new System.EventHandler(this.utmostToolStripMenuItem_Click);
            // 
            // setToolStripMenuItem
            // 
            this.setToolStripMenuItem.Name = "setToolStripMenuItem";
            this.setToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.setToolStripMenuItem.Text = "Set";
            this.setToolStripMenuItem.Click += new System.EventHandler(this.setToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 653);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(834, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mapToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeFileToolStripMenuItem,
            this.openMapToolStripMenuItem,
            this.saveMapToolStripMenuItem,
            this.exportBackgroundToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // analyzeFileToolStripMenuItem
            // 
            this.analyzeFileToolStripMenuItem.Name = "analyzeFileToolStripMenuItem";
            this.analyzeFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.analyzeFileToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.analyzeFileToolStripMenuItem.Text = "&Analyze File";
            this.analyzeFileToolStripMenuItem.Click += new System.EventHandler(this.analyzeFileToolStripMenuItem_Click);
            // 
            // openMapToolStripMenuItem
            // 
            this.openMapToolStripMenuItem.Name = "openMapToolStripMenuItem";
            this.openMapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMapToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openMapToolStripMenuItem.Text = "&Open map";
            this.openMapToolStripMenuItem.Click += new System.EventHandler(this.openMapToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.saveMapToolStripMenuItem.Text = "&Save map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // exportBackgroundToolStripMenuItem
            // 
            this.exportBackgroundToolStripMenuItem.Name = "exportBackgroundToolStripMenuItem";
            this.exportBackgroundToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportBackgroundToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.exportBackgroundToolStripMenuItem.Text = "E&xport background";
            this.exportBackgroundToolStripMenuItem.Click += new System.EventHandler(this.exportBackgroundToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetAllToolStripMenuItem,
            this.resetAnalyzedToolStripMenuItem,
            this.resizeToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.mapToolStripMenuItem.Text = "&Map";
            // 
            // resetAllToolStripMenuItem
            // 
            this.resetAllToolStripMenuItem.Name = "resetAllToolStripMenuItem";
            this.resetAllToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.resetAllToolStripMenuItem.Text = "&New Map";
            this.resetAllToolStripMenuItem.Click += new System.EventHandler(this.resetAllToolStripMenuItem_Click);
            // 
            // resetAnalyzedToolStripMenuItem
            // 
            this.resetAnalyzedToolStripMenuItem.Name = "resetAnalyzedToolStripMenuItem";
            this.resetAnalyzedToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.resetAnalyzedToolStripMenuItem.Text = "&Reset analyzed";
            this.resetAnalyzedToolStripMenuItem.Click += new System.EventHandler(this.resetAnalyzedToolStripMenuItem_Click);
            // 
            // resizeToolStripMenuItem
            // 
            this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
            this.resizeToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.resizeToolStripMenuItem.Text = "Re&size";
            this.resizeToolStripMenuItem.Click += new System.EventHandler(this.resizeToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.refreshToolStripMenuItem.Text = "Re&fresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRock,
            this.toolStripTunnel,
            this.toolStripReinforced,
            this.toolStripOre});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(834, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripRock
            // 
            this.toolStripRock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRock.Image = global::AnalyzeTool.Properties.Resources.rock;
            this.toolStripRock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRock.Name = "toolStripRock";
            this.toolStripRock.Size = new System.Drawing.Size(23, 22);
            this.toolStripRock.Text = "toolStripButton1";
            this.toolStripRock.ToolTipText = "Rock";
            this.toolStripRock.Click += new System.EventHandler(this.toolStripRock_Click);
            // 
            // toolStripTunnel
            // 
            this.toolStripTunnel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripTunnel.Image = global::AnalyzeTool.Properties.Resources.slab;
            this.toolStripTunnel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTunnel.Name = "toolStripTunnel";
            this.toolStripTunnel.Size = new System.Drawing.Size(23, 22);
            this.toolStripTunnel.Text = "toolStripButton2";
            this.toolStripTunnel.ToolTipText = "Tunnel";
            this.toolStripTunnel.Click += new System.EventHandler(this.toolStripTunnel_Click);
            // 
            // toolStripReinforced
            // 
            this.toolStripReinforced.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripReinforced.Image = global::AnalyzeTool.Properties.Resources.reinforcedcave;
            this.toolStripReinforced.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripReinforced.Name = "toolStripReinforced";
            this.toolStripReinforced.Size = new System.Drawing.Size(23, 22);
            this.toolStripReinforced.Text = "Reinforced";
            this.toolStripReinforced.ToolTipText = "Reinforced";
            this.toolStripReinforced.Click += new System.EventHandler(this.toolStripReinforced_Click);
            // 
            // toolStripOre
            // 
            this.toolStripOre.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOre.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCopper,
            this.toolStripGold,
            this.toolStripIron,
            this.toolStripLead,
            this.toolStripMarble,
            this.toolStripSilver,
            this.toolStripSlate,
            this.toolStripTip,
            this.toolStripZinc});
            this.toolStripOre.Image = global::AnalyzeTool.Properties.Resources.ironore;
            this.toolStripOre.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOre.Name = "toolStripOre";
            this.toolStripOre.Size = new System.Drawing.Size(29, 22);
            this.toolStripOre.Text = "toolStripDropDownButton1";
            this.toolStripOre.ToolTipText = "Ore";
            // 
            // toolStripCopper
            // 
            this.toolStripCopper.Image = global::AnalyzeTool.Properties.Resources.copperore;
            this.toolStripCopper.Name = "toolStripCopper";
            this.toolStripCopper.Size = new System.Drawing.Size(109, 22);
            this.toolStripCopper.Text = "Copper";
            this.toolStripCopper.Click += new System.EventHandler(this.copperToolStripMenuItem_Click);
            // 
            // toolStripGold
            // 
            this.toolStripGold.Image = global::AnalyzeTool.Properties.Resources.goldore;
            this.toolStripGold.Name = "toolStripGold";
            this.toolStripGold.Size = new System.Drawing.Size(109, 22);
            this.toolStripGold.Text = "Gold";
            this.toolStripGold.Click += new System.EventHandler(this.toolStripGold_Click);
            // 
            // toolStripIron
            // 
            this.toolStripIron.Image = global::AnalyzeTool.Properties.Resources.ironore;
            this.toolStripIron.Name = "toolStripIron";
            this.toolStripIron.Size = new System.Drawing.Size(109, 22);
            this.toolStripIron.Text = "Iron";
            this.toolStripIron.Click += new System.EventHandler(this.toolStripIron_Click);
            // 
            // toolStripLead
            // 
            this.toolStripLead.Image = global::AnalyzeTool.Properties.Resources.leadore;
            this.toolStripLead.Name = "toolStripLead";
            this.toolStripLead.Size = new System.Drawing.Size(109, 22);
            this.toolStripLead.Text = "Lead";
            this.toolStripLead.Click += new System.EventHandler(this.toolStripLead_Click);
            // 
            // toolStripMarble
            // 
            this.toolStripMarble.Image = global::AnalyzeTool.Properties.Resources.marbleshards;
            this.toolStripMarble.Name = "toolStripMarble";
            this.toolStripMarble.Size = new System.Drawing.Size(109, 22);
            this.toolStripMarble.Text = "Marble";
            this.toolStripMarble.Click += new System.EventHandler(this.toolStripMarble_Click);
            // 
            // toolStripSilver
            // 
            this.toolStripSilver.Image = global::AnalyzeTool.Properties.Resources.silverore;
            this.toolStripSilver.Name = "toolStripSilver";
            this.toolStripSilver.Size = new System.Drawing.Size(109, 22);
            this.toolStripSilver.Text = "Silver";
            this.toolStripSilver.Click += new System.EventHandler(this.toolStripSilver_Click);
            // 
            // toolStripSlate
            // 
            this.toolStripSlate.Image = global::AnalyzeTool.Properties.Resources.slateshards;
            this.toolStripSlate.Name = "toolStripSlate";
            this.toolStripSlate.Size = new System.Drawing.Size(109, 22);
            this.toolStripSlate.Text = "Slate";
            this.toolStripSlate.Click += new System.EventHandler(this.toolStripSlate_Click);
            // 
            // toolStripTip
            // 
            this.toolStripTip.Image = global::AnalyzeTool.Properties.Resources.tinore;
            this.toolStripTip.Name = "toolStripTip";
            this.toolStripTip.Size = new System.Drawing.Size(109, 22);
            this.toolStripTip.Text = "Tin";
            this.toolStripTip.Click += new System.EventHandler(this.toolStripTip_Click);
            // 
            // toolStripZinc
            // 
            this.toolStripZinc.Image = global::AnalyzeTool.Properties.Resources.zincore;
            this.toolStripZinc.Name = "toolStripZinc";
            this.toolStripZinc.Size = new System.Drawing.Size(109, 22);
            this.toolStripZinc.Text = "Zinc";
            this.toolStripZinc.Click += new System.EventHandler(this.toolStripZinc_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 675);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resultsBox);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "AnalyzeTool";
            this.panel1.ResumeLayout(false);
            this.gridContextMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox resultsBox;
        private System.Windows.Forms.Panel panel1;
        private GridControl gridControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyzeFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripRock;
        private System.Windows.Forms.ToolStripButton toolStripTunnel;
        private System.Windows.Forms.ToolStripDropDownButton toolStripOre;
        private System.Windows.Forms.ToolStripMenuItem toolStripCopper;
        private System.Windows.Forms.ToolStripMenuItem toolStripGold;
        private System.Windows.Forms.ToolStripMenuItem toolStripIron;
        private System.Windows.Forms.ToolStripMenuItem toolStripLead;
        private System.Windows.Forms.ToolStripMenuItem toolStripMarble;
        private System.Windows.Forms.ToolStripMenuItem toolStripSilver;
        private System.Windows.Forms.ToolStripMenuItem toolStripSlate;
        private System.Windows.Forms.ToolStripMenuItem toolStripTip;
        private System.Windows.Forms.ToolStripMenuItem toolStripZinc;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAnalyzedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip gridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem rockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tunnelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ironToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem marbleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem silverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zincToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem poorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acceptableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goodToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem veryGoodToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripReinforced;
        private System.Windows.Forms.ToolStripMenuItem reinforcedToolStripMenuItem;
    }
}

