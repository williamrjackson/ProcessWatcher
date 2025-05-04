using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Wrj.ProcessEnforcerTray
{
    internal class ProcessListing
    {
        private string path;
        private int delay;
        private Process _process;
        private Process process
        {
            get
            {
                if (_process == null)
                {
                    CreateNewProcess();
                }
                return _process;
            }
        }
        public enum ProcessState
        {
            Running,
            Launching,
            Stopped
        }
        public string FilePath => path;
        public string FileName => Path.GetFileNameWithoutExtension(path).ToLower();

        public int Delay
        {
            get => delay; set => delay = Math.Max(0, value);
        }

        private void CreateNewProcess()
        {
            //try
            //{
            //    if (_process != null)
            //    {
            //        if (!process.CloseMainWindow())
            //        {
            //            process.Kill();
            //        }
            //        _process.WaitForExit();
            //    }
            //}
            //catch 
            //{
            //    // Handle any exceptions that occur while closing the process
            //    ErrorLogging.Log($"Error closing process: {path}");
            //}
            //finally
            //{
                _process = new Process();
                _process.StartInfo.FileName = path;
                _process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                _process.StartInfo.UseShellExecute = true;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.Arguments = arguments;
            //}
        }
        public ProcessListing(string path, int delay)
        {
            this.path = path.Trim();
            Delay = delay;
            CreateNewProcess();
        }
        private string arguments;
        public string Arguments
        {
            get => arguments;
            set 
            {
                arguments = value;
            }
        }
        public ProcessState State
        {
            get
            {
                if (!IsRunning()) return ProcessState.Stopped;

                if (process == null || process.HasExited)
                {
                    return ProcessState.Stopped;
                }
                else if (process.StartTime.AddSeconds(delay) > DateTime.Now)
                {
                    return ProcessState.Launching;
                }
                else
                {
                    return ProcessState.Running;
                }
            }
        }
        public int AgeInSeconds()
        {
            try
            {
                if (process == null || process.HasExited)
                {
                    return 0;
                }
                TimeSpan age = DateTime.Now - process.StartTime;
                return (int)age.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsRunning()
        {
            bool isRunning = false;

            if (process == null) isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    _process = p;
                    break;
                }
            }
            if (!isRunning) CreateNewProcess();
            return isRunning;
        }
        public void Stop()
        {
            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    Logging.Log($"Stopping process: {p.ProcessName}");
                    try
                    {
                        if (!p.CloseMainWindow())
                        {
                            p.Kill();
                        }
                        p.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur while closing the process
                        Logging.Log($"Error stopping process: {ex.Message}");
                    }
                }
            }
        }
        public void Start()
        {
            if (!IsRunning())
            {
                Logging.Log($"Starting process: {path}");
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    Logging.Log($"Error starting process: {ex.Message}");
                }
            }
        }
    }
}
