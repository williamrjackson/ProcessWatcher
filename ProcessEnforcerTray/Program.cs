using System;
using System.IO;
using System.Windows.Forms;

namespace Wrj.ProcessEnforcerTray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check if an argument is provided and points to a valid file
            string alternativePath = args.Length > 0 && File.Exists(args[0]) ? args[0] : null;

            // Pass the alternative path to MainForm
            Application.Run(new MainForm(alternativePath));
        }
    }
}
