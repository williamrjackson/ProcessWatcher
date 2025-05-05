namespace ProcessEnforcerTray
{
    partial class UdpSettingsForm
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
            this.addressLabel = new System.Windows.Forms.Label();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.okayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(13, 13);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(68, 20);
            this.addressLabel.TabIndex = 0;
            this.addressLabel.Text = "Address";
            // 
            // addressTextBox
            // 
            this.addressTextBox.Location = new System.Drawing.Point(12, 36);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(284, 26);
            this.addressTextBox.TabIndex = 1;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(12, 90);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(69, 26);
            this.portTextBox.TabIndex = 3;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(13, 67);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(38, 20);
            this.portLabel.TabIndex = 2;
            this.portLabel.Text = "Port";
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(114, 120);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(75, 37);
            this.okayButton.TabIndex = 4;
            this.okayButton.Text = "Okay";
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // UdpSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 169);
            this.Controls.Add(this.okayButton);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.addressTextBox);
            this.Controls.Add(this.addressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UdpSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UDP Settings";
            this.Load += new System.EventHandler(this.UdpSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Button okayButton;
    }
}