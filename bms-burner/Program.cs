using System;
using System.Windows.Forms;

using Serilog;

namespace bms_burner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                try
                {
                    System.IO.File.Delete("bms-burner-log.txt");
                }
                catch (System.IO.FileNotFoundException) { }

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("bms-burner-log.txt", buffered: false)
                    .MinimumLevel.Verbose()
                    .CreateLogger();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal error in Main()");
                MessageBox.Show(ex.Message, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}
