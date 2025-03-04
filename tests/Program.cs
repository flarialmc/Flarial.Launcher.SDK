using System;
using System.IO;
using System.Windows.Forms;
using Flarial.Launcher;

static class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form());
    }
}