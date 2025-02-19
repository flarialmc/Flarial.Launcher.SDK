﻿using System;
using System.IO;
using System.Windows.Forms;
using Flarial.Launcher;

static class Program
{
    static void Main()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Flarial.Launcher");
        Directory.CreateDirectory(path); Directory.SetCurrentDirectory(path);

        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form());
    }
}