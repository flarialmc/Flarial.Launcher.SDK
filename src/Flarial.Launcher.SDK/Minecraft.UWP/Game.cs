namespace Minecraft.UWP;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Management.Core;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Linq;

/// <summary>
/// Provides methods to interact with Minecraft.
/// </summary>
public static class Game
{
    internal static readonly Win32Exception ERROR_PROCESS_ABORTED = new(0x0000042B);

    static async Task<App> GetAsync() => await App.GetAsync("Microsoft.MinecraftUWP_8wekyb3d8bbwe");

    /// <summary>
    /// Asynchronously obtain Minecraft's installed version.
    /// </summary>
    /// <returns>The version of Minecraft installed.</returns>
    public static async Task<string> VersionAsync()
    {
        var package = (await GetAsync()).Package;
        return await Task.Run(() =>
        {
            var path = XElement.Parse(File.ReadAllText(Path.Combine(package.InstalledPath, "AppxManifest.xml"))).Descendants().First(_ => _.Name.LocalName is "Application").Attribute("Executable").Value;
            var version = FileVersionInfo.GetVersionInfo(Path.Combine(package.InstalledPath, path)).FileVersion;
            return version.Substring(0, version.LastIndexOf('.'));
        });
    }

    /// <summary>
    /// Launches Minecraft &#38; waits for it to fully initialize.
    /// </summary>
    /// <returns>The PID of the game.</returns>
    public static async Task<int> LaunchAsync() => await Launch(true);

    internal static async Task<int> Launch(bool _)
    {
        var app = await GetAsync();

        var path = ApplicationDataManager.CreateForPackageFamily(app.Package.Id.FamilyName).LocalFolder.Path;
        TaskCompletionSource<bool> source = new();
        if (app.Running && !File.Exists(Path.Combine(path, @"games\com.mojang\minecraftpe\resource_init_lock"))) source.TrySetResult(true);

        using FileSystemWatcher watcher = new(path)
        {
            NotifyFilter = NotifyFilters.FileName,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };
        watcher.Deleted += (_, e) =>
        {
            if (e.Name.Equals(@"games\com.mojang\minecraftpe\resource_init_lock", StringComparison.OrdinalIgnoreCase))
                source.TrySetResult(true);
        };

        using var process = _ ? await Task.Run(app.Launch) : app.Launch();
        process.EnableRaisingEvents = true; process.Exited += (_, _) => throw ERROR_PROCESS_ABORTED; await source.Task;
        return process.Id;
    }

    /// <summary>
    ///  Asynchronously terminate Minecraft.
    /// </summary>
    public static async Task TerminateAsync() => await Task.Run(async () => (await GetAsync()).Terminate());
}