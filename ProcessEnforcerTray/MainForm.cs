using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Wrj.ProcessEnforcerTray
{
    public partial class MainForm : Form
    {
        private static System.Timers.Timer timer;
        private static List<string> processPaths = new List<string>();
        private static string persistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "processPaths.txt");
        private static string initBrowseDir = AppDomain.CurrentDomain.BaseDirectory;
        string persistFileText = string.Empty;

        private HashSet<string> runningProcs = new HashSet<string>();
        private bool AllRunning => runningProcs.Count > 0 && runningProcs.Count == processPaths.Count;
        private bool IsScanning => timer != null && timer.Enabled;

        private const string RegistryPath = @"Software\Wrj\ProcessEnforcer";
        private const string EnforceOrderSettingName = "EnforceOrder";
        private RegistryKey settingsKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
        public bool EnforceOrder
        {
            get
            {
                if (settingsKey != null)
                {
                    bool enforceOrderValue = Convert.ToBoolean(settingsKey.GetValue(EnforceOrderSettingName, launchOrderToggle.Checked));
                }
                return false;
            }
            set
            {
                settingsKey.SetValue(EnforceOrderSettingName, value);
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void PathsDataChanged()
        {
            processListBox.Items.Clear();
            persistFileText = string.Empty;
            
            if (processPaths.Count > 0)
            {
                for (int i = 0; i < processPaths.Count; i++)
                {
                    processListBox.Items.Add(processPaths[i]);
                    persistFileText += $"{processPaths[i]}{Environment.NewLine}";
                }
                File.WriteAllText(persistPath, persistFileText.Trim(Environment.NewLine.ToCharArray()));
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //launchOrderToggle.Checked = EnforceOrder;

            if (File.Exists(persistPath))
            {
                persistFileText = File.ReadAllText(persistPath);
                processPaths = new List<string>(persistFileText.Trim(Environment.NewLine.ToCharArray()).Split(Environment.NewLine.ToCharArray()));
                processPaths.RemoveAll((s) => string.IsNullOrWhiteSpace(s) || !File.Exists(s));
                PathsDataChanged();
            }
        }

        private void MinimizeToTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            this.Invoke(new MethodInvoker(delegate () {
                Hide();
                WindowState = FormWindowState.Minimized;
                notifyIcon.Visible = true;
            }));
        }

        private void OpenFromTray()
        {
            if (!this.IsHandleCreated) CreateHandle();
            this.Invoke(new MethodInvoker(delegate () {
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }));
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

        private static bool ProcessIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    break;
                }
            }
            return isRunning;
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
            if (timer != null && timer.Enabled)
            {
                //Console.WriteLine("Timer Stopped");
                scanButton.Text = "Scan";
                timer.Enabled = false;
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("Timer!");
            foreach (var path in processPaths)
            {
                bool isRunning = ProcessIsRunning(path);
                if (!isRunning)
                {
                    runningProcs.Remove(path);
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = path
                    };
                    Process.Start(startInfo);
                }
                else
                {
                    runningProcs.Add(path);
                }
            }

            if (AllRunning)
            {
                MinimizeToTray();
            }
            else
            {
                OpenFromTray();
            }
        }
        private void PersistPaths()
        {
            persistFileText = string.Empty;
            for (int i = 0; i < processPaths.Count; i++)
            {
                string lineEnd = (i == processPaths.Count - 1) ? "\n" : "";
                persistFileText += $"{processPaths[i]}{lineEnd}";
            }
            File.WriteAllText(persistPath, persistFileText);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            fileDialog.InitialDirectory = initBrowseDir;
            fileDialog.Filter = "exe files (*.exe)|*.exe";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                initBrowseDir = Path.GetDirectoryName(fileDialog.FileName);
                AddExePath(fileDialog.FileName);
            }
        }
        private void AddExePath(string path)
        {
            if (path.EndsWith(".exe") && File.Exists(path))
            {
                if (!processPaths.Contains(path, StringComparer.InvariantCultureIgnoreCase))
                {
                    processPaths.Add(path);
                    PathsDataChanged();
                }
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
                StartTimer();
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (processListBox.SelectedItem == null) return;
            string toRemove = (string)processListBox.SelectedItem;
            if (processPaths.Remove(toRemove))
            {
                runningProcs.Remove(toRemove);
                PathsDataChanged();
            }
        }

        private void launchOrderToggle_CheckedChanged(object sender, EventArgs e)
        {
            EnforceOrder = launchOrderToggle.Checked;
        }
    }
}