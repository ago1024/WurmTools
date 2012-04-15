namespace MiningRatio
{
    partial class MiningRatioForm
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
            this.playerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.wurmFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.startScan = new System.Windows.Forms.Button();
            this.logText = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblActions = new System.Windows.Forms.Label();
            this.lblTicks = new System.Windows.Forms.Label();
            this.lblRatio = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblSkillLast = new System.Windows.Forms.Label();
            this.lblSkillTicks = new System.Windows.Forms.Label();
            this.lblSkillActions = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblSkillTime = new System.Windows.Forms.Label();
            this.lblTotalSkill = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playerName
            // 
            this.playerName.Location = new System.Drawing.Point(12, 25);
            this.playerName.Name = "playerName";
            this.playerName.Size = new System.Drawing.Size(100, 20);
            this.playerName.TabIndex = 0;
            this.playerName.TextChanged += new System.EventHandler(this.playerName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Player name";
            // 
            // wurmFolder
            // 
            this.wurmFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wurmFolder.CausesValidation = false;
            this.wurmFolder.Location = new System.Drawing.Point(118, 25);
            this.wurmFolder.Name = "wurmFolder";
            this.wurmFolder.Size = new System.Drawing.Size(391, 20);
            this.wurmFolder.TabIndex = 2;
            this.wurmFolder.TextChanged += new System.EventHandler(this.wurmFolder_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Wurm folder";
            // 
            // startScan
            // 
            this.startScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startScan.Location = new System.Drawing.Point(515, 23);
            this.startScan.Name = "startScan";
            this.startScan.Size = new System.Drawing.Size(75, 23);
            this.startScan.TabIndex = 4;
            this.startScan.Text = "Reset";
            this.startScan.UseVisualStyleBackColor = true;
            this.startScan.Click += new System.EventHandler(this.startScan_Click);
            // 
            // logText
            // 
            this.logText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logText.Location = new System.Drawing.Point(12, 115);
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            this.logText.Size = new System.Drawing.Size(578, 304);
            this.logText.TabIndex = 5;
            this.logText.Text = "";
            this.logText.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mining actions";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Skill ticks";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Ratio";
            // 
            // lblActions
            // 
            this.lblActions.AutoSize = true;
            this.lblActions.Location = new System.Drawing.Point(96, 52);
            this.lblActions.Name = "lblActions";
            this.lblActions.Size = new System.Drawing.Size(13, 13);
            this.lblActions.TabIndex = 9;
            this.lblActions.Text = "0";
            // 
            // lblTicks
            // 
            this.lblTicks.AutoSize = true;
            this.lblTicks.Location = new System.Drawing.Point(96, 69);
            this.lblTicks.Name = "lblTicks";
            this.lblTicks.Size = new System.Drawing.Size(13, 13);
            this.lblTicks.TabIndex = 10;
            this.lblTicks.Text = "0";
            // 
            // lblRatio
            // 
            this.lblRatio.AutoSize = true;
            this.lblRatio.Location = new System.Drawing.Point(96, 86);
            this.lblRatio.Name = "lblRatio";
            this.lblRatio.Size = new System.Drawing.Size(13, 13);
            this.lblRatio.TabIndex = 11;
            this.lblRatio.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(165, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Skill/h of last action";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(165, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Skill/h for ticks";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(165, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Skill/h for actions";
            // 
            // lblSkillLast
            // 
            this.lblSkillLast.AutoSize = true;
            this.lblSkillLast.Location = new System.Drawing.Point(271, 52);
            this.lblSkillLast.Name = "lblSkillLast";
            this.lblSkillLast.Size = new System.Drawing.Size(13, 13);
            this.lblSkillLast.TabIndex = 15;
            this.lblSkillLast.Text = "0";
            // 
            // lblSkillTicks
            // 
            this.lblSkillTicks.AutoSize = true;
            this.lblSkillTicks.Location = new System.Drawing.Point(271, 69);
            this.lblSkillTicks.Name = "lblSkillTicks";
            this.lblSkillTicks.Size = new System.Drawing.Size(13, 13);
            this.lblSkillTicks.TabIndex = 16;
            this.lblSkillTicks.Text = "0";
            // 
            // lblSkillActions
            // 
            this.lblSkillActions.AutoSize = true;
            this.lblSkillActions.Location = new System.Drawing.Point(271, 86);
            this.lblSkillActions.Name = "lblSkillActions";
            this.lblSkillActions.Size = new System.Drawing.Size(13, 13);
            this.lblSkillActions.TabIndex = 17;
            this.lblSkillActions.Text = "0";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(450, 86);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(13, 13);
            this.lblTotalTime.TabIndex = 23;
            this.lblTotalTime.Text = "0";
            // 
            // lblSkillTime
            // 
            this.lblSkillTime.AutoSize = true;
            this.lblSkillTime.Location = new System.Drawing.Point(450, 69);
            this.lblSkillTime.Name = "lblSkillTime";
            this.lblSkillTime.Size = new System.Drawing.Size(13, 13);
            this.lblSkillTime.TabIndex = 22;
            this.lblSkillTime.Text = "0";
            // 
            // lblTotalSkill
            // 
            this.lblTotalSkill.AutoSize = true;
            this.lblTotalSkill.Location = new System.Drawing.Point(450, 52);
            this.lblTotalSkill.Name = "lblTotalSkill";
            this.lblTotalSkill.Size = new System.Drawing.Size(13, 13);
            this.lblTotalSkill.TabIndex = 21;
            this.lblTotalSkill.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(344, 86);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Action time overall";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(344, 69);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Action time for ticks";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(344, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(83, 13);
            this.label14.TabIndex = 18;
            this.label14.Text = "Skill gain overall";
            // 
            // MiningRatioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(602, 431);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.lblSkillTime);
            this.Controls.Add(this.lblTotalSkill);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lblSkillActions);
            this.Controls.Add(this.lblSkillTicks);
            this.Controls.Add(this.lblSkillLast);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblRatio);
            this.Controls.Add(this.lblTicks);
            this.Controls.Add(this.lblActions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.startScan);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wurmFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerName);
            this.Name = "MiningRatioForm";
            this.Text = "Wurm Mining Ratio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox playerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox wurmFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startScan;
        private System.Windows.Forms.RichTextBox logText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblActions;
        private System.Windows.Forms.Label lblTicks;
        private System.Windows.Forms.Label lblRatio;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblSkillLast;
        private System.Windows.Forms.Label lblSkillTicks;
        private System.Windows.Forms.Label lblSkillActions;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblSkillTime;
        private System.Windows.Forms.Label lblTotalSkill;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
    }
}

