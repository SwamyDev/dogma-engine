namespace PontAnimator
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showcase = new System.Windows.Forms.PictureBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txbContentPath = new System.Windows.Forms.TextBox();
            this.btnSelectContentFolder = new System.Windows.Forms.Button();
            this.lblSelectAnimation = new System.Windows.Forms.Label();
            this.cobAnimations = new System.Windows.Forms.ComboBox();
            this.lblFrames = new System.Windows.Forms.Label();
            this.txbFrames = new System.Windows.Forms.TextBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txbFramesPerSecond = new System.Windows.Forms.TextBox();
            this.lblFramesPerSecond = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.lblAnimationName = new System.Windows.Forms.Label();
            this.txbAnimationName = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.chbShowCollisionShape = new System.Windows.Forms.CheckBox();
            this.pnlColorPicker = new System.Windows.Forms.Panel();
            this.lblBackgroundColor = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.prbExport = new System.Windows.Forms.ProgressBar();
            this.lblColumn = new System.Windows.Forms.Label();
            this.txbColumn = new System.Windows.Forms.TextBox();
            this.btnExportSpriteSheet = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showcase)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(880, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // showcase
            // 
            this.showcase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.showcase.Location = new System.Drawing.Point(271, 71);
            this.showcase.Name = "showcase";
            this.showcase.Size = new System.Drawing.Size(600, 450);
            this.showcase.TabIndex = 1;
            this.showcase.TabStop = false;
            this.showcase.Resize += new System.EventHandler(this.showcase_Resize);
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(268, 28);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(79, 13);
            this.lblContent.TabIndex = 2;
            this.lblContent.Text = "Content folder: ";
            // 
            // txbContentPath
            // 
            this.txbContentPath.Location = new System.Drawing.Point(271, 44);
            this.txbContentPath.Name = "txbContentPath";
            this.txbContentPath.Size = new System.Drawing.Size(560, 20);
            this.txbContentPath.TabIndex = 6;
            // 
            // btnSelectContentFolder
            // 
            this.btnSelectContentFolder.Location = new System.Drawing.Point(837, 44);
            this.btnSelectContentFolder.Name = "btnSelectContentFolder";
            this.btnSelectContentFolder.Size = new System.Drawing.Size(29, 20);
            this.btnSelectContentFolder.TabIndex = 7;
            this.btnSelectContentFolder.Text = "...";
            this.btnSelectContentFolder.UseVisualStyleBackColor = true;
            this.btnSelectContentFolder.Click += new System.EventHandler(this.btnSelectContentFolder_Click);
            // 
            // lblSelectAnimation
            // 
            this.lblSelectAnimation.AutoSize = true;
            this.lblSelectAnimation.Location = new System.Drawing.Point(13, 28);
            this.lblSelectAnimation.Name = "lblSelectAnimation";
            this.lblSelectAnimation.Size = new System.Drawing.Size(88, 13);
            this.lblSelectAnimation.TabIndex = 5;
            this.lblSelectAnimation.Text = "Select animation:";
            // 
            // cobAnimations
            // 
            this.cobAnimations.FormattingEnabled = true;
            this.cobAnimations.Location = new System.Drawing.Point(16, 44);
            this.cobAnimations.Name = "cobAnimations";
            this.cobAnimations.Size = new System.Drawing.Size(246, 21);
            this.cobAnimations.TabIndex = 1;
            this.cobAnimations.SelectedIndexChanged += new System.EventHandler(this.cobAnimations_SelectedIndexChanged);
            // 
            // lblFrames
            // 
            this.lblFrames.AutoSize = true;
            this.lblFrames.Location = new System.Drawing.Point(16, 100);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(41, 13);
            this.lblFrames.TabIndex = 7;
            this.lblFrames.Text = "Frames";
            // 
            // txbFrames
            // 
            this.txbFrames.Location = new System.Drawing.Point(123, 97);
            this.txbFrames.Name = "txbFrames";
            this.txbFrames.Size = new System.Drawing.Size(138, 20);
            this.txbFrames.TabIndex = 3;
            this.txbFrames.TextChanged += new System.EventHandler(this.txbFrames_TextChanged);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(271, 528);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(36, 23);
            this.btnPlay.TabIndex = 8;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(367, 528);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(41, 23);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txbFramesPerSecond
            // 
            this.txbFramesPerSecond.Location = new System.Drawing.Point(123, 124);
            this.txbFramesPerSecond.Name = "txbFramesPerSecond";
            this.txbFramesPerSecond.Size = new System.Drawing.Size(138, 20);
            this.txbFramesPerSecond.TabIndex = 4;
            this.txbFramesPerSecond.TextChanged += new System.EventHandler(this.txbFramesPerSecond_TextChanged);
            // 
            // lblFramesPerSecond
            // 
            this.lblFramesPerSecond.AutoSize = true;
            this.lblFramesPerSecond.Location = new System.Drawing.Point(16, 127);
            this.lblFramesPerSecond.Name = "lblFramesPerSecond";
            this.lblFramesPerSecond.Size = new System.Drawing.Size(102, 13);
            this.lblFramesPerSecond.TabIndex = 12;
            this.lblFramesPerSecond.Text = "Frames per Second:";
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(313, 528);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(48, 23);
            this.btnPause.TabIndex = 9;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // lblAnimationName
            // 
            this.lblAnimationName.AutoSize = true;
            this.lblAnimationName.Location = new System.Drawing.Point(16, 74);
            this.lblAnimationName.Name = "lblAnimationName";
            this.lblAnimationName.Size = new System.Drawing.Size(85, 13);
            this.lblAnimationName.TabIndex = 14;
            this.lblAnimationName.Text = "Animation name:";
            // 
            // txbAnimationName
            // 
            this.txbAnimationName.Location = new System.Drawing.Point(123, 71);
            this.txbAnimationName.Name = "txbAnimationName";
            this.txbAnimationName.Size = new System.Drawing.Size(138, 20);
            this.txbAnimationName.TabIndex = 2;
            this.txbAnimationName.TextChanged += new System.EventHandler(this.txbAnimationName_TextChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(19, 174);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(142, 23);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export configured stripes";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // chbShowCollisionShape
            // 
            this.chbShowCollisionShape.AutoSize = true;
            this.chbShowCollisionShape.Checked = true;
            this.chbShowCollisionShape.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbShowCollisionShape.Location = new System.Drawing.Point(746, 532);
            this.chbShowCollisionShape.Name = "chbShowCollisionShape";
            this.chbShowCollisionShape.Size = new System.Drawing.Size(125, 17);
            this.chbShowCollisionShape.TabIndex = 11;
            this.chbShowCollisionShape.Text = "Show collision shape";
            this.chbShowCollisionShape.UseVisualStyleBackColor = true;
            // 
            // pnlColorPicker
            // 
            this.pnlColorPicker.BackColor = System.Drawing.Color.Black;
            this.pnlColorPicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColorPicker.Location = new System.Drawing.Point(703, 530);
            this.pnlColorPicker.Name = "pnlColorPicker";
            this.pnlColorPicker.Size = new System.Drawing.Size(26, 19);
            this.pnlColorPicker.TabIndex = 19;
            this.pnlColorPicker.Click += new System.EventHandler(this.pnlColorPicker_Click);
            // 
            // lblBackgroundColor
            // 
            this.lblBackgroundColor.AutoSize = true;
            this.lblBackgroundColor.Location = new System.Drawing.Point(603, 533);
            this.lblBackgroundColor.Name = "lblBackgroundColor";
            this.lblBackgroundColor.Size = new System.Drawing.Size(94, 13);
            this.lblBackgroundColor.TabIndex = 20;
            this.lblBackgroundColor.Text = "Background color:";
            // 
            // prbExport
            // 
            this.prbExport.Location = new System.Drawing.Point(16, 567);
            this.prbExport.Name = "prbExport";
            this.prbExport.Size = new System.Drawing.Size(161, 23);
            this.prbExport.TabIndex = 12;
            // 
            // lblColumn
            // 
            this.lblColumn.AutoSize = true;
            this.lblColumn.Location = new System.Drawing.Point(174, 297);
            this.lblColumn.Name = "lblColumn";
            this.lblColumn.Size = new System.Drawing.Size(45, 13);
            this.lblColumn.TabIndex = 21;
            this.lblColumn.Text = "Column:";
            // 
            // txbColumn
            // 
            this.txbColumn.Location = new System.Drawing.Point(225, 294);
            this.txbColumn.Name = "txbColumn";
            this.txbColumn.Size = new System.Drawing.Size(36, 20);
            this.txbColumn.TabIndex = 22;
            this.txbColumn.TextChanged += new System.EventHandler(this.txbColumn_TextChanged);
            // 
            // btnExportSpriteSheet
            // 
            this.btnExportSpriteSheet.Location = new System.Drawing.Point(123, 321);
            this.btnExportSpriteSheet.Name = "btnExportSpriteSheet";
            this.btnExportSpriteSheet.Size = new System.Drawing.Size(137, 58);
            this.btnExportSpriteSheet.TabIndex = 23;
            this.btnExportSpriteSheet.Text = "Bernhard\'s magical button you\'re never allowed to press. Never!";
            this.btnExportSpriteSheet.UseVisualStyleBackColor = true;
            this.btnExportSpriteSheet.Click += new System.EventHandler(this.btnExportSpriteSheet_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 602);
            this.Controls.Add(this.btnExportSpriteSheet);
            this.Controls.Add(this.txbColumn);
            this.Controls.Add(this.lblColumn);
            this.Controls.Add(this.prbExport);
            this.Controls.Add(this.lblBackgroundColor);
            this.Controls.Add(this.pnlColorPicker);
            this.Controls.Add(this.chbShowCollisionShape);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.txbAnimationName);
            this.Controls.Add(this.lblAnimationName);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.lblFramesPerSecond);
            this.Controls.Add(this.txbFramesPerSecond);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.txbFrames);
            this.Controls.Add(this.lblFrames);
            this.Controls.Add(this.cobAnimations);
            this.Controls.Add(this.lblSelectAnimation);
            this.Controls.Add(this.btnSelectContentFolder);
            this.Controls.Add(this.txbContentPath);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.showcase);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Infamus Pont Animator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showcase)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        public System.Windows.Forms.PictureBox showcase;
        private System.Windows.Forms.Label lblContent;
        public System.Windows.Forms.TextBox txbContentPath;
        private System.Windows.Forms.Button btnSelectContentFolder;
        private System.Windows.Forms.Label lblSelectAnimation;
        public System.Windows.Forms.ComboBox cobAnimations;
        private System.Windows.Forms.Label lblFrames;
        private System.Windows.Forms.TextBox txbFrames;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txbFramesPerSecond;
        private System.Windows.Forms.Label lblFramesPerSecond;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Label lblAnimationName;
        private System.Windows.Forms.TextBox txbAnimationName;
        private System.Windows.Forms.Button btnExport;
        public System.Windows.Forms.CheckBox chbShowCollisionShape;
        private System.Windows.Forms.Panel pnlColorPicker;
        private System.Windows.Forms.Label lblBackgroundColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ProgressBar prbExport;
        private System.Windows.Forms.Label lblColumn;
        private System.Windows.Forms.TextBox txbColumn;
        private System.Windows.Forms.Button btnExportSpriteSheet;
    }
}