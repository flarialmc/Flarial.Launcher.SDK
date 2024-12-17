namespace Minecraft.UWP;

using System.Linq;
using Windows.System;
using System.Diagnostics;
using Windows.ApplicationModel;

sealed class App(string _)
{
    const int AO_NOERRORUI = 0x00000002;

    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    static readonly PackageDebugSettings PackageDebugSettings = new();

    readonly AppInfo AppInfo = AppInfo.GetFromAppUserModelId(_);

    internal Package Package => AppInfo.Package;

    internal bool Running => AppDiagnosticInfo.RequestInfoForAppAsync(AppInfo.AppUserModelId).Get().SelectMany(_ => _.GetResourceGroups().SelectMany(_ => _.GetProcessDiagnosticInfos())).Any();

    internal Process Launch()
    {
        PackageDebugSettings.EnableDebugging(Package.Id.FullName, default, default);
        ApplicationActivationManager.ActivateApplication(AppInfo.AppUserModelId, default, AO_NOERRORUI, out var processId);
        return Process.GetProcessById(processId);
    }

    internal void Terminate() => PackageDebugSettings.TerminateAllProcesses(Package.Id.FullName);
}