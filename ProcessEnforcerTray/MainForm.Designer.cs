namespace Wrj.ProcessEnforcerTray
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
            this.processListBox = new System.Windows.Forms.ListBox();
            this.scanButton = new System.Windows.Forms.Button();
            this.launchOrderToggle = new System.Windows.Forms.CheckBox();
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
            this.browseBtn.Location = new System.Drawing.Point(825, 11);
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
            this.removeButton.Location = new System.Drawing.Point(825, 61);
            this.removeButton.Margin = new System.Windows.Forms.Padding(4);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(112, 33);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // processListBox
            // 
            this.processListBox.FormattingEnabled = true;
            this.processListBox.ItemHeight = 20;
            this.processListBox.Location = new System.Drawing.Point(18, 21);
            this.processListBox.Margin = new System.Windows.Forms.Padding(4);
            this.processListBox.Name = "processListBox";
            this.processListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.processListBox.Size = new System.Drawing.Size(794, 204);
            this.processListBox.TabIndex = 4;
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(825, 190);
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
            this.launchOrderToggle.Checked = true;
            this.launchOrderToggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.launchOrderToggle.Enabled = false;
            this.launchOrderToggle.Location = new System.Drawing.Point(18, 213);
            this.launchOrderToggle.Name = "launchOrderToggle";
            this.launchOrderToggle.Size = new System.Drawing.Size(192, 24);
            this.launchOrderToggle.TabIndex = 6;
            this.launchOrderToggle.Text = "Enforce Launch Order";
            this.launchOrderToggle.UseVisualStyleBackColor = true;
            this.launchOrderToggle.Visible = false;
            this.launchOrderToggle.CheckedChanged += new System.EventHandler(this.launchOrderToggle_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 240);
            this.Controls.Add(this.launchOrderToggle);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.processListBox);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.browseBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Process Enforcer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ListBox processListBox;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.CheckBox launchOrderToggle;
    }
}

