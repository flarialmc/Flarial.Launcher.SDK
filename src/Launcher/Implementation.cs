using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

public static partial class Launcher
{
    [DllImport("Kernel32")]
    static extern uint GetCurrentProcessId();

    static Launcher()
    {
        var assembly = Assembly.GetEntryAssembly();
        Version = assembly.GetName().Version;
        Executable = assembly.ManifestModule.FullyQualifiedName;

        using StreamReader stream = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("Script.cmd"));
        Script = stream.ReadToEnd();
    }

    static readonly Version Version;

    static readonly string Script;

    static readonly string Executable;

    public static partial async Task<bool> CheckAsync() => new Version((await Web.LauncherAsync())["version"].GetString()) == Version;

    public static async partial Task UpdateAsync(Action<int> action)
    {
        var path = Path.GetTempPath();
        var executable = Path.Combine(path, Path.GetRandomFileName());
        var script = Path.Combine(path, Path.ChangeExtension(Path.GetRandomFileName(), ".cmd"));

        await Web.DownloadAsync((await Web.LauncherAsync())["downloadUrl"].GetString(), executable, action);

        using StreamWriter stream = new(script);
        await stream.WriteAsync(string.Format(Script, GetCurrentProcessId(), executable, Executable));

        using (Process.Start(new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "conhost.exe"),
            Arguments = $"--headless \"{script}\"",
            UseShellExecute = true
        })) { }
    }
}