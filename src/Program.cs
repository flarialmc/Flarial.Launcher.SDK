using System;
using System.IO;
using System.Windows.Forms;

static class Program
{
    static void Main()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Flarial.Launcher");
        Directory.CreateDirectory(path); Directory.SetCurrentDirectory(path);

        AppDomain.CurrentDomain.UnhandledException += (_, e) => Logger.Log(e.ExceptionObject.ToString());

        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.Run(new Form());
    }
}