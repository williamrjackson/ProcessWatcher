using System;
using System.IO;
using System.Windows.Forms;

namespace ProcessEnforcerTray
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

            // Pass the alternative path to MainForm
            Application.Run(new MainForm(args));
        }
    }
}
