using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

static class Logger
{
    static Logger()
    {
        Directory.CreateDirectory("Logs");
        Trace.Listeners.AddRange((TextWriterTraceListener[])[new(Console.Out), new(File.CreateText(@$"Logs\{DateTime.UtcNow.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture)}.txt"))]);
        Trace.AutoFlush = true;
    }

    internal static void Log(string value) => Trace.WriteLine($"[{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}] {value}");
}