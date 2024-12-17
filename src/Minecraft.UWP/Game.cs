namespace Minecraft.UWP;

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Management.Core;

/// <summary>
/// Provides methods to interact with Minecraft.
/// </summary>
public static class Game
{
    static readonly App App = new("Microsoft.MinecraftUWP_8wekyb3d8bbwe!App");

    /// <summary>
    /// Asynchronously obtain Minecraft's installed version.
    /// </summary>
    /// <returns>The version of Minecraft installed.</returns>
    public static async Task<string> VersionAsync() => await Task.Run(() =>
    {
        var path = App.Package.InstalledPath;
        using var stream = File.OpenRead(Path.Combine(path, "AppxManifest.xml"));
        var _ = FileVersionInfo.GetVersionInfo(Path.Combine(path, XElement.Load(stream).Descendants().First(_ => _.Name.LocalName is "Application").Attribute("Executable").Value)).FileVersion;
        return _.Substring(0, _.LastIndexOf('.'));
    });

    internal static int Launch()
    {
        var path = ApplicationDataManager.CreateForPackageFamily(App.Package.Id.FamilyName).LocalFolder.Path;
        using ManualResetEventSlim @event = new(App.Running && !File.Exists(Path.Combine(path, @"games\com.mojang\minecraftpe\resource_init_lock")));

        using FileSystemWatcher watcher = new(path) { NotifyFilter = NotifyFilters.FileName, IncludeSubdirectories = true, EnableRaisingEvents = true };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(@"games\com.mojang\minecraftpe\resource_init_lock", StringComparison.OrdinalIgnoreCase))
         @event.Set(); };

        using var process = App.Launch();
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => throw new OperationCanceledException();

        @event.Wait(); return process.Id;
    }

    internal static void Terminate() => App.Terminate();

    /// <summary>
    /// Asynchronously launches Minecraft &#38; waits for it to fully initialize.
    /// </summary>
    /// <returns>The PID of the game.</returns>
    public static async Task LaunchAsync() => await Task.Run(Launch);

    /// <summary>
    ///  Asynchronously terminate Minecraft.
    /// </summary>
    public static async Task TerminateAsync() => await Task.Run(Terminate);
}