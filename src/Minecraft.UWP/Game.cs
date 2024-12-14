namespace Minecraft.UWP;

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Management.Core;

/// <summary>
/// Provides methods to interact with Minecraft.
/// </summary>
public static class Game
{
    static readonly Win32Exception ERROR_PROCESS_ABORTED = new(0x0000042B);

    internal static async Task<App> GetAsync() => await App.GetAsync(Global.PackageFamilyName);

    /// <summary>
    /// Asynchronously obtain Minecraft's installed version.
    /// </summary>
    /// <returns>The version of Minecraft installed.</returns>
    public static async Task<string> VersionAsync() => await Task.Run(async () =>
    {
        var path = (await GetAsync()).Package.InstalledPath;
        using var stream = File.OpenRead(Path.Combine(path, "AppxManifest.xml"));
        var _ = FileVersionInfo.GetVersionInfo(Path.Combine(path, XElement.Load(stream).Descendants().First(_ => _.Name.LocalName is "Application").Attribute("Executable").Value)).FileVersion;
        return _.Substring(0, _.LastIndexOf('.'));
    });

    /// <summary>
    /// Launches Minecraft &#38; waits for it to fully initialize.
    /// </summary>
    /// <returns>The PID of the game.</returns>
    public static async Task<int> LaunchAsync() => await Task.Run(() => Launch());

    internal static async Task<int> Launch(App _ = default)
    {
        _ ??= await GetAsync();

        TaskCompletionSource<bool> source = new(); var path = ApplicationDataManager.CreateForPackageFamily(_.Package.Id.FamilyName).LocalFolder.Path;
        if (_.Running && !File.Exists(Path.Combine(path, @"games\com.mojang\minecraftpe\resource_init_lock"))) source.TrySetResult(true);

        using FileSystemWatcher watcher = new(path) { NotifyFilter = NotifyFilters.FileName, IncludeSubdirectories = true, EnableRaisingEvents = true };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(@"games\com.mojang\minecraftpe\resource_init_lock", StringComparison.OrdinalIgnoreCase)) source.TrySetResult(true); };

        using var process = _.Launch();
        process.EnableRaisingEvents = true; process.Exited += (_, _) => throw ERROR_PROCESS_ABORTED; await source.Task;
        return process.Id;
    }

    /// <summary>
    ///  Asynchronously terminate Minecraft.
    /// </summary>
    public static async Task TerminateAsync() => await Task.Run(async () => (await GetAsync()).Terminate());
}