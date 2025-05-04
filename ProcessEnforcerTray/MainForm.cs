using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;

namespace Wrj.ProcessEnforcerTray
{
    public partial class MainForm : Form
    {
        private static System.Timers.Timer timer;
        private static List<ProcessListing> processPaths = new List<ProcessListing>();
        private static string persistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "processPaths.txt");
        private static string initBrowseDir = AppDomain.CurrentDomain.BaseDirectory;
        private TextBox editTextBox = new TextBox();
        string persistFileText = string.Empty;

        private bool IsScanning => timer != null && timer.Enabled;
        private bool IsEditing = false;
        private bool IsSizeChanging = false;

        private const string RegistryPath = @"Software\Wrj\ProcessEnforcer";
        private const string EnforceOrderSettingName = "EnforceOrder";
        private RegistryKey settingsKey = Registry.CurrentUser.CreateSubKey(RegistryPath);

        private bool enforceOrder = false;
        public bool EnforceOrder
        {
            get => enforceOrder;
            set
            {
                enforceOrder = value;
                settingsKey.SetValue(EnforceOrderSettingName, value);
            }
        }

        public MainForm()
        {
            InitializeComponent();

            enforceOrder = Convert.ToBoolean(settingsKey.GetValue(EnforceOrderSettingName, launchOrderToggle.Checked));

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
                    persistFileText += $"{processPaths[i].FilePath};{processPaths[i].Arguments};{processPaths[i].Delay}{Environment.NewLine}";
                }
                File.WriteAllText(persistPath, persistFileText.Trim(Environment.NewLine.ToCharArray()));
                if (!IsEditing) StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            launchOrderToggle.Checked = EnforceOrder;
            launchOrderToggle.Enabled = settingsKey != null;

            if (File.Exists(persistPath))
            {
                persistFileText = File.ReadAllText(persistPath);
                //processPaths = new List<ProcessListing>(persistFileText.Trim(Environment.NewLine.ToCharArray()).Split(Environment.NewLine.ToCharArray()));
                processPaths = new List<ProcessListing>();
                string[] lines = persistFileText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int delay))
                    {
                        ProcessListing newProcess = new ProcessListing(parts[0].Trim(), delay);
                        newProcess.Arguments = parts[1].Trim();
                        processPaths.Add(newProcess);

                    }
                }
                processPaths.RemoveAll((s) => string.IsNullOrWhiteSpace(s.FilePath) || !File.Exists(s.FilePath));
                PathsDataChanged();
            }
        }

        private void MinimizeToTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            this.Invoke(new MethodInvoker(delegate ()
            {
                Hide();
                WindowState = FormWindowState.Minimized;
                notifyIcon.Visible = true;
            }));
        }

        private void OpenFromTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            this.Invoke(new MethodInvoker(delegate ()
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }));
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
            foreach (var path in processPaths)
            {
                path.Stop();
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
                            //Console.WriteLine($"Stopping {processPaths[i].FilePath} ({processPaths[i].AgeInSeconds()}) because it was launched before {processPaths[i - 1].FilePath}({processPaths[i - 1].AgeInSeconds()})");
                            CloseAll();
                            OpenFromTray();
                            return;
                        }
                    }
                    MinimizeToTray();
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
                processPaths.Remove(processPaths.First(proc => proc.FilePath == toRemove));
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
                //Console.WriteLine($"Clicked column {hitTest.Item.SubItems.IndexOf(hitTest.SubItem)}: {hitTest.SubItem.Text}");
                //Console.WriteLine($"Clicked row {processListView.Items.IndexOf(hitTest.Item)}");
                int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
                if (columnIndex == 0) return;
                int rowIndex = processListView.Items.IndexOf(hitTest.Item);

                //Console.WriteLine($"Editing column {columnIndex}");
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

    }
}