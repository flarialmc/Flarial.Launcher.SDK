namespace Minecraft.UWP;

using System.Linq;
using Windows.System;
using System.Threading;
using System.Diagnostics;
using Windows.Foundation;
using Windows.ApplicationModel;
using System.Collections.Generic;

sealed class App(string _)
{
    const int AO_NOERRORUI = 0x00000002;

    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    static readonly PackageDebugSettings PackageDebugSettings = new();

    readonly AppInfo AppInfo = AppInfo.GetFromAppUserModelId(_);

    internal Package Package => AppInfo.Package;

    internal IEnumerable<int> Processes
    {
        get
        {
            var _ = AppDiagnosticInfo.RequestInfoForAppAsync(AppInfo.AppUserModelId); try
            {
                if (_.Status is AsyncStatus.Started)
                {
                    using ManualResetEventSlim @event = new();
                    _.Completed += (_, _) => @event.Set(); @event.Wait();
                }
                return (_.Status is AsyncStatus.Error ? throw _.ErrorCode : _.GetResults())
                .SelectMany(_ => _.GetResourceGroups().SelectMany(_ => _.GetProcessDiagnosticInfos().Select(_ => (int)_.ProcessId)));
            }
            finally { _.Close(); }
        }
    }

    internal Process Launch()
    {
        PackageDebugSettings.EnableDebugging(Package.Id.FullName, default, default);
        ApplicationActivationManager.ActivateApplication(AppInfo.AppUserModelId, default, AO_NOERRORUI, out var processId);
        return Process.GetProcessById(processId);
    }

    internal void Terminate() => PackageDebugSettings.TerminateAllProcesses(Package.Id.FullName);
}