namespace Minecraft.UWP;

using System;
using System.IO;
using System.Linq;
using Windows.System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Management.Core;
using Windows.ApplicationModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// Provides method to interact with Minecraft.
/// </summary>
public static class Game
{
    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    static readonly PackageDebugSettings PackageDebugSettings = new();

    internal const int ERROR_INSTALL_PACKAGE_NOT_FOUND = unchecked((int)0x80073CF1);

    internal const int ERROR_INSTALL_WRONG_PROCESSOR_ARCHITECTURE = unchecked((int)0x80073D10);

    internal const int AO_NOERRORUI = 0x00000002;

    static Package Package()
    {
        var package = Global.PackageManager.FindPackagesForUser(string.Empty, "Microsoft.MinecraftUWP_8wekyb3d8bbwe").FirstOrDefault();

        if (package is null) Marshal.ThrowExceptionForHR(ERROR_INSTALL_PACKAGE_NOT_FOUND);
        else if (package.Id.Architecture is not ProcessorArchitecture.X64) Marshal.ThrowExceptionForHR(ERROR_INSTALL_WRONG_PROCESSOR_ARCHITECTURE);

        return package;
    }

    /// <summary>
    /// Launches Minecraft &#38; waits for it to fully initialize.
    /// </summary>
    /// <returns>The PID of the game.</returns>
    public static int Activate()
    {
        var package = Package();

        Marshal.ThrowExceptionForHR(PackageDebugSettings.DisableDebugging(package.Id.FullName));
        Marshal.ThrowExceptionForHR(PackageDebugSettings.GetPackageExecutionState(package.Id.FullName, out var packageExecutionState));
        Marshal.ThrowExceptionForHR(PackageDebugSettings.EnableDebugging(package.Id.FullName, default, default));

        var path = ApplicationDataManager.CreateForPackageFamily(package.Id.FamilyName).LocalFolder.Path;
        var state = packageExecutionState is not (PackageExecutionState.Unknown or PackageExecutionState.Terminated);
        if (state) state = !File.Exists(Path.Combine(path, @"games\com.mojang\minecraftpe\resource_init_lock"));

        using ManualResetEventSlim @event = new(state);
        using FileSystemWatcher watcher = new(path) { NotifyFilter = NotifyFilters.FileName, IncludeSubdirectories = true, EnableRaisingEvents = true };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(@"games\com.mojang\minecraftpe\resource_init_lock", StringComparison.OrdinalIgnoreCase)) @event.Set(); };

        Marshal.ThrowExceptionForHR(ApplicationActivationManager.ActivateApplication("Microsoft.MinecraftUWP_8wekyb3d8bbwe!App", null, AO_NOERRORUI, out var processId));

        using var process = Process.GetProcessById(processId);
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => @event.Set();

        @event.Wait(); return processId;
    }

    /// <summary>
    /// Terminates Minecraft.
    /// </summary>
    public static void Terminate() => PackageDebugSettings.TerminateAllProcesses(Package().Id.FullName);

    /// <summary>
    /// Asynchronously launches Minecraft &#38; waits for it to fully initialize.
    /// </summary>
    /// <returns>The PID of the game.</returns>
    public static async Task<int> ActivateAsync() => await Task.Run(Activate).ConfigureAwait(false);

    /// <summary>
    ///  Asynchronously terminates Minecraft.
    /// </summary>
    public static async Task TerminateAsync() => await Task.Run(Terminate).ConfigureAwait(false);
}