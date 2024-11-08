using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

static class Logger
{
    static readonly StreamWriter Writer;

    static readonly object _ = new();

    static Logger()
    {
        Directory.CreateDirectory("Logs");
        Writer = new(new FileStream(@$"Logs\{DateTime.UtcNow.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture)}.txt", FileMode.Create, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
    }

    internal static void Log(string value)
    {
        lock (_)
        {
            var _ = $"[{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}] {value}";
            Task.WhenAll(Writer.WriteLineAsync(_), Console.Out.WriteLineAsync(_));
        }
    }
}