using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Net;

namespace ProcessEnforcerTray
{
    public partial class MainForm : Form
    {
        private static System.Timers.Timer timer;
        private static List<ProcessListing> processPaths = new List<ProcessListing>();
        private static string persistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "processPaths.txt");
        private static string initBrowseDir = AppDomain.CurrentDomain.BaseDirectory;
        private TextBox editTextBox = new TextBox();
        string persistFileText = string.Empty;

        public string PersistPath
        {
            get
            {
                if (!IsFilePathWritable(persistPath))
                {
                    persistPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Process Enforcer", Path.GetFileName(persistPath));
                }
                return persistPath;
            }
        }
        private bool IsScanning => timer != null && timer.Enabled;
        private bool IsEditing = false;
        private bool IsSizeChanging = false;
        private bool isUsingAlternativePath = false;

        private const string RegistryPath = @"Software\Wrj\ProcessEnforcer";
        private const string EnforceOrderSettingName = "EnforceOrder";
        private const string UdpAddressSettingName = "UdpAddress";
        private const string UdpPortSettingName = "UdpPort";
        private static RegistryKey settingsKey = Registry.CurrentUser.CreateSubKey(RegistryPath);

        private static UDPSocket udpServer = new UDPSocket();

        private bool enforceOrder = false;
        private static string udpAddress = "127.0.0.1";
        private static int udpPort = 27000;
        public static IPAddress UdpAddress
        {
            get
            {
                if (IPAddress.TryParse(udpAddress, out var ip))
                {
                    return ip;
                }
                return null;
            }
            private set
            {
                udpAddress = value.ToString();
                try
                {
                    settingsKey.SetValue(UdpAddressSettingName, value);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logging.Log($"Error accessing registry: {ex.Message}");
                }
            }
        }
        public static int UdpPort
        {
            get => udpPort;
            private set
            {
                if (value >= 0 || value <= 65535)
                {
                    udpPort = value;
                    try
                    {
                        settingsKey.SetValue(UdpPortSettingName, value);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Logging.Log($"Error accessing registry: {ex.Message}");
                    }
                }
            }
        }

        public bool EnforceOrder
        {
            get => enforceOrder;
            set
            {
                enforceOrder = value;
                try
                {
                    settingsKey.SetValue(EnforceOrderSettingName, value);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logging.Log($"Error accessing registry: {ex.Message}");
                }
            }
        }

        public MainForm(string[] args)
        {

            InitializeComponent();

            processListView.Columns[0].Width = processListView.Bounds.Width / 2;
            processListView.Columns[1].Width = processListView.Bounds.Width / 2;
            processListView.Columns[2].Width = 0;
            processListView.FullRowSelect = true;
            processListView.LabelEdit = false;
            processListView.View = View.Details;
            processListView.MouseClick += ProcessListView_MouseClick;
            processListView.ColumnWidthChanging += ProcessListView_ColumnWidthChanging;
            processListView.AllowDrop = true;
            processListView.ItemDrag += ProcessListView_ItemDrag;
            processListView.DragEnter += ProcessListView_DragEnter;
            processListView.DragDrop += ProcessListView_DragDrop;

            // Add the TextBox for editing
            editTextBox.Visible = false;
            editTextBox.Leave += EditTextBox_Leave;
            editTextBox.KeyPress += EditTextBox_KeyPress;
            editTextBox.KeyDown += EditTextBox_KeyDown;
            processListView.Controls.Add(editTextBox);

            if (args.Length > 0 && File.Exists(args[0]))
            {
                LoadLauncherFile(args[0], true);
            }
            else if (args.Length > 1)
            {
                if (args.Where(x => x.Split(',').Length == 3).Count() == args.Length)
                {
                    ParseProcessPaths(args);
                }
            }
        }

        private void ShowHideDelayColumn()
        {
            IsSizeChanging = true;
            if (EnforceOrder)
            {
                processListView.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
                processListView.Columns[0].Width = (processListView.Bounds.Width - processListView.Columns[2].Width) / 2;
                processListView.Columns[1].Width = (processListView.Bounds.Width - processListView.Columns[2].Width) / 2;
            }
            else
            {
                processListView.Columns[0].Width = processListView.Bounds.Width / 2;
                processListView.Columns[1].Width = processListView.Bounds.Width / 2;
                processListView.Columns[2].Width = 0;
            }
            IsSizeChanging = false;
        }

        private void PathsDataChanged()
        {
            processListView.Items.Clear();
            persistFileText = string.Empty;

            if (processPaths.Count > 0)
            {
                for (int i = 0; i < processPaths.Count; i++)
                {
                    processListView.Items.Add(new ListViewItem(new string[] { processPaths[i].FilePath, processPaths[i].Arguments, processPaths[i].Delay.ToString() }));
                    persistFileText += $"{processPaths[i].FilePath},{processPaths[i].Arguments},{processPaths[i].Delay}{Environment.NewLine}";
                }
                File.WriteAllText(PersistPath, persistFileText.Trim(Environment.NewLine.ToCharArray()));
                if (!IsEditing) StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!isUsingAlternativePath)
            {
                enforceOrder = Convert.ToBoolean(settingsKey.GetValue(EnforceOrderSettingName, launchOrderToggle.Checked));
            }
            launchOrderToggle.Checked = EnforceOrder;
            launchOrderToggle.Enabled = settingsKey != null;

            udpPort = Convert.ToInt32(settingsKey.GetValue(UdpPortSettingName, udpPort));
            udpAddress = settingsKey.GetValue(UdpAddressSettingName, udpAddress).ToString();
            if (IPAddress.TryParse(udpAddress, out var ip))
            {
                UdpAddress = ip;
            }
            else
            {
                Logging.Log($"Invalid IP address format in registry: {udpAddress}");
                UdpAddress = IPAddress.Loopback;
            }
            udpServer.MessageReceived += UdpServer_MessageReceived;
            StartUdpServer(UdpAddress, UdpPort);
            Logging.Log("Loading settings from registry");

            if ((processPaths == null || processPaths.Count == 0) && File.Exists(PersistPath))
            {
                LoadLauncherFile(PersistPath, false);
            }
        }

        private void UdpServer_MessageReceived(string msg, EndPoint endPoint)
        {
            Logging.Log($"UDP Message Received: {msg} from {endPoint}");
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    ParseUdpMessage(msg);
                }));
            }
            else
            {
                ParseUdpMessage(msg);
            }
        }
        private void ParseUdpMessage(string msg)
        {
            msg = msg.Trim();

            string[] parts = msg.Split(',');
            if (parts.Length % 3 == 0)
            {
                for (int i = 0; i < parts.Length; i += 3)
                {
                    if (!File.Exists(parts[i]))
                    {
                        Logging.Log($"Invalid file path provided: {parts[i]}");
                        return;
                    }
                }
                lock (processPaths)
                {
                    processPaths.Clear();
                    for (int i = 0; i < parts.Length; i += 3)
                    {
                        string path = parts[i].Trim();
                        string args = parts[i + 1].Trim();
                        int delay = int.TryParse(parts[i + 2], out int d) ? d : 0;
                        ProcessListing newProcess = new ProcessListing(path, delay);
                        newProcess.Arguments = args;
                        processPaths.Add(newProcess);
                    }
                }
                PathsDataChanged();
            }
            else if (parts.Length == 1 && msg.EndsWith(".txt"))
            {
                LoadLauncherFile(msg, true);
            }
        }

        private void ParseProcessPaths(string[] lines)
        {
            CloseAll();
            lock (processPaths)
            {
                processPaths = new List<ProcessListing>();
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int delay))
                    {
                        ProcessListing newProcess = new ProcessListing(parts[0].Trim(), delay);
                        newProcess.Arguments = parts[1].Trim();
                        processPaths.Add(newProcess);
                    }
                }
            }
            PathsDataChanged();
        }
        private void MinimizeToTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    Hide();
                    WindowState = FormWindowState.Minimized;
                    notifyIcon.Visible = true;
                }));
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                notifyIcon.Visible = true;
            }
        }

        private void OpenFromTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            if (this.InvokeRequired)
            {

                this.Invoke(new MethodInvoker(delegate ()
                {
                    Show();
                    this.WindowState = FormWindowState.Normal;
                    notifyIcon.Visible = false;
                }));
            }
            else
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }
        }
        private void LoadLauncherFile(string path, bool isAlt)
        {
            if (!File.Exists(path))
            {
                // Try BaseDirectory
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }
            }
            if (File.Exists(path) && Path.GetExtension(path).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                persistPath = path;
            }
            else
            {
                Logging.Log($"Invalid file path provided: {path}");
                return;
            }

            persistPath = Path.GetFullPath(path);
            if (isAlt)
            {
                Logging.Log($"Alternative path provided: {persistPath}");
                isUsingAlternativePath = true;
                enforceOrder = true;
            }
            string[] lines = File.ReadAllLines(persistPath);
            ParseProcessPaths(lines);
        }

        public static void StartUdpServer(IPAddress address, int port)
        {
            UdpAddress = address;
            UdpPort = port;
            try
            {
                udpServer.Server(address, port);
            }
            catch (Exception ex)
            {
                Logging.Log($"Error starting UDP server: {ex.Message}");
            }
        }
        private void StartTimer()
        {
            if (timer == null || !timer.Enabled)
            {
                //Console.WriteLine("Timer Started");
                scanButton.Text = "Stop";
                timer = new System.Timers.Timer(5000);
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
        }
        private void StopTimer()
        {
            //Console.WriteLine("Timer Stopped");
            if (timer != null && timer.Enabled)
            {
                scanButton.Text = "Scan";
                timer.Enabled = false;
            }
        }

        private void CloseAll()
        {
            lock (processPaths)
            {
                foreach (var path in processPaths)
                {
                    path.Stop();
                }
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (EnforceOrder)
            {
                if (processPaths.Count == 0) return;
                if (processPaths.Count > 1)
                {
                    for (int i = processPaths.Count - 1; i >= 1; i--)
                    {
                        if (processPaths[i].AgeInSeconds() > processPaths[i - 1].AgeInSeconds())
                        {
                            Logging.Log($"Stopping {processPaths[i].FilePath} ({processPaths[i].AgeInSeconds()}) because it was launched before {processPaths[i - 1].FilePath}({processPaths[i - 1].AgeInSeconds()})");
                            CloseAll();
                            OpenFromTray();
                            return;
                        }
                    }
                }
                for (int i = 0; i < processPaths.Count; i++)
                {
                    if (processPaths[i].State == ProcessListing.ProcessState.Running)
                    {
                        if (i == processPaths.Count - 1)
                        {
                            MinimizeToTray();
                            continue;
                        }
                    }
                    else if (processPaths[i].State == ProcessListing.ProcessState.Stopped)
                    {
                        processPaths[i].Start();
                        return;
                    }
                    else //Launching
                    {
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < processPaths.Count; i++)
                {
                    if (processPaths[i].State == ProcessListing.ProcessState.Running)
                    {
                        if (i == processPaths.Count - 1)
                            MinimizeToTray();
                        continue;
                    }
                    else if (processPaths[i].State == ProcessListing.ProcessState.Stopped)
                    {
                        OpenFromTray();
                        processPaths[i].Start();
                        return;
                    }
                    else //Launching
                    {
                        return;
                    }
                }
            }
        }

        private void AddExePath(string path)
        {
            if (File.Exists(path))
            {
                // Check if the path already exists in the list
                if (!processPaths.Any(e => e.FilePath == path))
                {
                    processPaths.Add(new ProcessListing(path, 0));
                    PathsDataChanged();
                }
            }
        }

        public bool IsFilePathWritable(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // If we can open the file for writing, it's writable
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Logging.Log($"Unauthorized access to file: {filePath}");
                return false; // No write permissions
            }
            catch (IOException)
            {
                Logging.Log($"File is locked or inaccessible: {filePath}");
                return false; // File is locked or inaccessible
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            IsEditing = true;
            StopTimer();
            fileDialog.InitialDirectory = initBrowseDir;
            fileDialog.Filter = "exe files (*.exe)|*.exe";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                initBrowseDir = Path.GetDirectoryName(fileDialog.FileName);
                AddExePath(fileDialog.FileName);
            }
        }
        private void scanButton_Click(object sender, EventArgs e)
        {
            if (IsScanning)
            {
                StopTimer();
            }
            else
            {
                IsEditing = false;
                StartTimer();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                StartTimer();
                MinimizeToTray();
            }
        }
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFromTray();
            StopTimer();
        }


        private void removeButton_Click(object sender, EventArgs e)
        {
            IsEditing = true;
            StopTimer();

            if (processListView.SelectedItems.Count == 0) return;

            foreach (var item in processListView.SelectedItems)
            {
                string toRemove = ((ListViewItem)item).SubItems[0].Text;
                processPaths.First(proc => proc.FilePath == toRemove).Stop();
                lock (processPaths)
                {
                    processPaths.Remove(processPaths.First(proc => proc.FilePath == toRemove));
                }
            }
            PathsDataChanged();
        }

        private void launchOrderToggle_CheckedChanged(object sender, EventArgs e)
        {
            EnforceOrder = launchOrderToggle.Checked;
            ShowHideDelayColumn();
        }

        private void ProcessListView_MouseClick(object sender, MouseEventArgs e)
        {
            var hitTest = processListView.HitTest(e.Location);
            if (hitTest.Item != null && hitTest.SubItem != null)
            {
                IsEditing = true;
                int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
                if (columnIndex == 0) return;
                int rowIndex = processListView.Items.IndexOf(hitTest.Item);

                Rectangle cellBounds = hitTest.SubItem.Bounds;
                editTextBox.Bounds = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height);
                editTextBox.Text = hitTest.SubItem.Text;
                hitTest.SubItem.Tag = new int[] { columnIndex, rowIndex };
                editTextBox.Tag = hitTest.SubItem; // Store the SubItem being edited
                editTextBox.Visible = true;
                editTextBox.Focus();
            }
        }
        private void EditTextBox_Leave(object sender, EventArgs e)
        {
            FinishEditing();
        }
        private void EditTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (editTextBox.Tag is ListViewItem.ListViewSubItem subItem)
            {
                if (subItem.Tag is int[] colRow)
                {
                    if (colRow[0] == 2) // Delay column
                    {
                        // Allow only digits and control characters (like backspace)
                        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                            e.Handled = true; // Block the input
                    }
                }
            }
        }
        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FinishEditing();
                e.Handled = true; // Prevent the beep sound
            }
            else if (e.KeyCode == Keys.Escape)
            {
                editTextBox.Visible = false;
                e.Handled = true; // Prevent the beep sound
            }
        }

        private void FinishEditing()
        {
            if (editTextBox.Tag is ListViewItem.ListViewSubItem subItem)
            {
                subItem.Text = editTextBox.Text; // Update the ListView with the new value
                if (subItem.Tag is int[] colRow)
                {
                    var arr = processPaths.ToArray();

                    if (colRow[0] == 1)
                    {
                        arr[colRow[1]].Arguments = subItem.Text;
                    }
                    else if (colRow[0] == 2)
                    {
                        int delay = 0;
                        int.TryParse(subItem.Text, out delay);
                        arr[colRow[1]].Delay = delay;
                    }

                    processPaths = new List<ProcessListing>(arr);
                    PathsDataChanged();
                }
            }
            editTextBox.Visible = false;
        }
        private void ProcessListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // If EnforceOrder is disabled, do nothing
            if (!EnforceOrder) return;
            StopTimer();
            IsEditing = true;

            // Start the drag-and-drop operation
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void ProcessListView_DragEnter(object sender, DragEventArgs e)
        {
            // Allow the drag operation
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ProcessListView_DragDrop(object sender, DragEventArgs e)
        {
            // If EnforceOrder is disabled, do nothing
            if (!EnforceOrder) return;
            StopTimer();
            IsEditing = true;
            // Get the dropped item
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                var draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                var targetPoint = processListView.PointToClient(new Point(e.X, e.Y));
                var targetItem = processListView.GetItemAt(targetPoint.X, targetPoint.Y);
                if (targetItem != null && draggedItem != targetItem)
                {
                    // Remove the dragged item and reinsert it at the target position
                    int targetIndex = targetItem.Index;
                    processListView.Items.Remove(draggedItem);
                    processListView.Items.Insert(targetIndex, draggedItem);

                    // Update the processPaths list based on the new order
                    processPaths = processListView.Items
                    .Cast<ListViewItem>()
                    .Select(item => processPaths.First(p => p.FilePath == item.Text))
                    .ToList();

                    PathsDataChanged();
                }
            }
        }
        private void ProcessListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (IsSizeChanging) return;

            e.Cancel = true; // Prevent the column width from changing
            e.NewWidth = processListView.Columns[e.ColumnIndex].Width; // Keep the current width
        }

        private void loadLaunchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsEditing = true;
            StopTimer();
            fileDialog.InitialDirectory = initBrowseDir;
            fileDialog.Filter = "txt files (*.txt)|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                initBrowseDir = Path.GetDirectoryName(fileDialog.FileName);
                LoadLauncherFile(fileDialog.FileName, true);
            }
        }

        private void uDPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UdpSettingsForm udpSettingsForm = new UdpSettingsForm();
            udpSettingsForm.ShowDialog();
        }
    }
}