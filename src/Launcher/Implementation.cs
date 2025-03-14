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
        Destination = assembly.ManifestModule.FullyQualifiedName;
    }

    static readonly Version Version;

    static readonly string Destination;

    static readonly string Temp = Path.GetTempPath();

    static readonly string System = Environment.GetFolderPath(Environment.SpecialFolder.System);

    static readonly string Content = $"\"{Path.Combine(System, "taskkill.exe")}\" /f /pid {{0}}\n:_\ncopy /y \"{{1}}\" \"{{2}}\"\nif not %errorlevel%==0 goto _\ndel \"%~f0\"";

    static readonly string File = Path.Combine(System, "cmd.exe");

    public static partial async Task<bool> AvailableAsync() => new Version((await Web.LauncherAsync())["version"].GetString()) != Version;

    public static async partial Task UpdateAsync(Action<int> action)
    {
        var source = Path.Combine(Temp, Path.GetRandomFileName());
        var path = Path.Combine(Temp, Path.ChangeExtension(Path.GetRandomFileName(), ".cmd"));

        await Web.DownloadAsync((await Web.LauncherAsync())["downloadUrl"].GetString(), source, action);

        using StreamWriter stream = new(path);
        await stream.WriteAsync(string.Format(Content, GetCurrentProcessId(), source, Destination));

        using (Process.Start(new ProcessStartInfo { 
            FileName = File,
            Arguments = $"/e:on /c call \"{path}\" & \"{File}\" /c start \"\" \"{Destination}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        })) { }
    }
}