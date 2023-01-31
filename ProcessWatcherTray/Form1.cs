using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace ProcessWatcherTray
{
    public partial class Form1 : Form
    {
        private static System.Timers.Timer timer;
        private static List<string> processPaths = new List<string>();
        private static string persistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "processPaths.txt");
        private static string initBrowseDir = AppDomain.CurrentDomain.BaseDirectory;
        string persistFileText = string.Empty;

        private HashSet<string> runningProcs = new HashSet<string>();
        private bool AllRunning => runningProcs.Count > 0 && runningProcs.Count == processPaths.Count;

        private bool IsScanning => timer != null && timer.Enabled;

        public Form1()
        {
            InitializeComponent();
        }

        private void PathsDataChanged()
        {
            listBox1.Items.Clear();
            persistFileText = string.Empty;
            
            if (processPaths.Count > 0)
            {
                for (int i = 0; i < processPaths.Count; i++)
                {
                    listBox1.Items.Add(processPaths[i]);
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

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void Form1_Resize(object sender, EventArgs e)
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
                button3.Text = "Stop";
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
                button3.Text = "Scan";
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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = initBrowseDir;
            openFileDialog1.Filter = "exe files (*.exe)|*.exe";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                initBrowseDir = Path.GetDirectoryName(openFileDialog1.FileName);
                AddExePath(openFileDialog1.FileName);
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

        private void button3_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            string toRemove = (string)listBox1.SelectedItem;
            if (processPaths.Remove(toRemove))
            {
                runningProcs.Remove(toRemove);
                PathsDataChanged();
            }
        }
    }
}