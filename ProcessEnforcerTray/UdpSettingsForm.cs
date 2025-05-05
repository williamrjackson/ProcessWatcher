using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessEnforcerTray
{
    public partial class UdpSettingsForm : Form
    {
        public UdpSettingsForm()
        {
            InitializeComponent();
        }

        private void UdpSettingsForm_Load(object sender, EventArgs e)
        {
            addressTextBox.Text = MainForm.UdpAddress.ToString();
            portTextBox.Text = MainForm.UdpPort.ToString();
            addressTextBox.KeyDown += AddressTextBox_KeyDown;
            addressTextBox.KeyPress += AddressTextBox_KeyPress;
            portTextBox.KeyDown += PortTextBox_KeyDown;
            portTextBox.KeyPress += PortTextBox_KeyPress;
        }
        private void Submit()
        {
            IPAddress ip = MainForm.UdpAddress;
            int prt = MainForm.UdpPort;
            if (IPAddress.TryParse(addressTextBox.Text, out ip))
            {
                if (int.TryParse(portTextBox.Text, out prt))
                {
                    if (prt < 0 || prt > 65535)
                    {
                        portTextBox.Select();
                        MessageBox.Show("Port number must be between 0 and 65535.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    portTextBox.Select();
                    MessageBox.Show("Invalid port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                addressTextBox.Select();
                MessageBox.Show("Invalid IP address format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MainForm.StartUdpServer(ip, prt);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void okayButton_Click(object sender, EventArgs e)
        {
            Submit();
        }
        private void AddressTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (like backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true; // Block the input
        }
        private void AddressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Submit();
                e.Handled = true; // Prevent the beep sound
            }
            else if (e.KeyCode == Keys.Escape)
            {
                portTextBox.Text = MainForm.UdpAddress.ToString();
                e.Handled = true; // Prevent the beep sound
            }
        }
        private void PortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (like backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true; // Block the input
        }
        private void PortTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Submit();
                e.Handled = true; // Prevent the beep sound
            }
            else if (e.KeyCode == Keys.Escape)
            {
                portTextBox.Text = MainForm.UdpPort.ToString();
                e.Handled = true; // Prevent the beep sound
            }
        }

    }
}
