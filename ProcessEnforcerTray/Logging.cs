using System;
using System.IO;

namespace ProcessEnforcerTray
{
    internal class Logging
    {
        private static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
        private static bool isInitialized = false;
        private static bool initializeAttempted = false;
        public static void Log(string message)
        {
            Console.WriteLine(message);
            if (!isInitialized && !initializeAttempted)
            {
                InitializeLog(logFilePath);
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur while writing to the log file
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
        private static void InitializeLog(string path)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                }
                File.WriteAllText(path, string.Empty);
                isInitialized = true;
            }
            catch
            {
                if (initializeAttempted)
                {
                    return;
                }
                string alt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Process Enforcer", "error_log.txt");
                if (path == alt)
                {
                    initializeAttempted = true;
                    return;
                }
                InitializeLog(alt);
            }
        }
    }
}
