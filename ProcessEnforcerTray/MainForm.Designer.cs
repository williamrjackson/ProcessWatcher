namespace ProcessEnforcerTray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.browseBtn = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.scanButton = new System.Windows.Forms.Button();
            this.launchOrderToggle = new System.Windows.Forms.CheckBox();
            this.processListView = new System.Windows.Forms.ListView();
            this.processPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.arguments = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.delayTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLaunchFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uDPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Process Enforcer";
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // fileDialog
            // 
            this.fileDialog.DefaultExt = "exe";
            // 
            // browseBtn
            // 
            this.browseBtn.Location = new System.Drawing.Point(825, 40);
            this.browseBtn.Margin = new System.Windows.Forms.Padding(4);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(112, 33);
            this.browseBtn.TabIndex = 1;
            this.browseBtn.Text = "Browse...";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(825, 90);
            this.removeButton.Margin = new System.Windows.Forms.Padding(4);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(112, 33);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(825, 223);
            this.scanButton.Margin = new System.Windows.Forms.Padding(4);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(112, 33);
            this.scanButton.TabIndex = 5;
            this.scanButton.Text = "Scan";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // launchOrderToggle
            // 
            this.launchOrderToggle.AutoSize = true;
            this.launchOrderToggle.Location = new System.Drawing.Point(18, 242);
            this.launchOrderToggle.Name = "launchOrderToggle";
            this.launchOrderToggle.Size = new System.Drawing.Size(192, 24);
            this.launchOrderToggle.TabIndex = 6;
            this.launchOrderToggle.Text = "Enforce Launch Order";
            this.launchOrderToggle.UseVisualStyleBackColor = true;
            this.launchOrderToggle.CheckedChanged += new System.EventHandler(this.launchOrderToggle_CheckedChanged);
            // 
            // processListView
            // 
            this.processListView.AllowDrop = true;
            this.processListView.AutoArrange = false;
            this.processListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.processPath,
            this.arguments,
            this.delayTime});
            this.processListView.FullRowSelect = true;
            this.processListView.GridLines = true;
            this.processListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.processListView.HideSelection = false;
            this.processListView.Location = new System.Drawing.Point(18, 40);
            this.processListView.Name = "processListView";
            this.processListView.Scrollable = false;
            this.processListView.ShowGroups = false;
            this.processListView.ShowItemToolTips = true;
            this.processListView.Size = new System.Drawing.Size(800, 196);
            this.processListView.TabIndex = 7;
            this.processListView.UseCompatibleStateImageBehavior = false;
            this.processListView.View = System.Windows.Forms.View.Details;
            // 
            // processPath
            // 
            this.processPath.Text = "Process";
            this.processPath.Width = 300;
            // 
            // arguments
            // 
            this.arguments.Text = "Args";
            // 
            // delayTime
            // 
            this.delayTime.Text = "Delay(sec)";
            this.delayTime.Width = 300;
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(951, 33);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadLaunchFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadLaunchFileToolStripMenuItem
            // 
            this.loadLaunchFileToolStripMenuItem.Name = "loadLaunchFileToolStripMenuItem";
            this.loadLaunchFileToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.loadLaunchFileToolStripMenuItem.Text = "Load Launch File...";
            this.loadLaunchFileToolStripMenuItem.Click += new System.EventHandler(this.loadLaunchFileToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uDPToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(92, 29);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // uDPToolStripMenuItem
            // 
            this.uDPToolStripMenuItem.Name = "uDPToolStripMenuItem";
            this.uDPToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.uDPToolStripMenuItem.Text = "UDP...";
            this.uDPToolStripMenuItem.Click += new System.EventHandler(this.uDPToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 273);
            this.Controls.Add(this.processListView);
            this.Controls.Add(this.launchOrderToggle);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Process Enforcer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.CheckBox launchOrderToggle;
        private System.Windows.Forms.ListView processListView;
        private System.Windows.Forms.ColumnHeader processPath;
        private System.Windows.Forms.ColumnHeader delayTime;
        private System.Windows.Forms.ColumnHeader arguments;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLaunchFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uDPToolStripMenuItem;
    }
}

