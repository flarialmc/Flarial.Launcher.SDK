namespace Minecraft;
using static Native;

using System;
using System.IO;
using System.Linq;
using Windows.System;
using System.Threading;
using System.ComponentModel;
using Windows.Management.Core;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using System.Runtime.InteropServices;

static class Game
{
    static readonly PackageManager PackageManager = new();

    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    static readonly PackageDebugSettings PackageDebugSettings = new();

    static Package Get()
    {
        var package = PackageManager.FindPackagesForUser(string.Empty, "Microsoft.MinecraftUWP_8wekyb3d8bbwe").FirstOrDefault();

        if (package is null) Marshal.ThrowExceptionForHR(ERROR_INSTALL_PACKAGE_NOT_FOUND);
        else if (package.Id.Architecture != RuntimeInformation.OSArchitecture switch
        {
            Architecture.X86 => ProcessorArchitecture.X86,
            Architecture.X64 => ProcessorArchitecture.X64,
            Architecture.Arm => ProcessorArchitecture.Arm,
            Architecture.Arm64 => ProcessorArchitecture.Arm64,
            _ => ProcessorArchitecture.Unknown
        }) Marshal.ThrowExceptionForHR(ERROR_INSTALL_WRONG_PROCESSOR_ARCHITECTURE);

        return package;
    }


    internal static int Launch()
    {
        var package = Get();

        Marshal.ThrowExceptionForHR(PackageDebugSettings.EnableDebugging(package.Id.FullName, default, default));
        using ManualResetEventSlim @event = new();
        using FileSystemWatcher watcher = new(ApplicationDataManager.CreateForPackageFamily(package.Id.FamilyName).LocalFolder.Path)
        {
            NotifyFilter = NotifyFilters.FileName,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };
        watcher.Deleted += (_, e) => { if (e.Name.Equals(@"games\com.mojang\minecraftpe\resource_init_lock", StringComparison.OrdinalIgnoreCase)) @event.Set(); };

        Marshal.ThrowExceptionForHR(ApplicationActivationManager.ActivateApplication(package.GetAppListEntries()[0].AppUserModelId, null, AO_NOERRORUI, out var processId));
        @event.Wait(); return processId;
    }

    internal static void Terminate() => PackageDebugSettings.TerminateAllProcesses(Get().Id.FullName);
}