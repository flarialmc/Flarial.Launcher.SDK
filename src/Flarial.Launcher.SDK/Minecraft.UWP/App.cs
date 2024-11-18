namespace Minecraft.UWP;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;
using static Unmanaged;

sealed class App
{
    AppDiagnosticInfo AppDiagnosticInfo;

    static readonly ApplicationActivationManager ApplicationActivationManager = new();

    static readonly PackageDebugSettings PackageDebugSettings = new();

    static readonly Win32Exception ERROR_INSTALL_PACKAGE_NOT_FOUND = new(unchecked((int)0x80073CF1));

    static readonly Win32Exception ERROR_INSTALL_WRONG_PROCESSOR_ARCHITECTURE = new(unchecked((int)0x80073D10));

    const int AO_NOERRORUI = 0x00000002;

    public static async Task<App> GetAsync(string packageFamilyName)
    {
        var _ = (await AppDiagnosticInfo.RequestInfoForPackageAsync(packageFamilyName)).FirstOrDefault();

        if (_ is null) throw ERROR_INSTALL_PACKAGE_NOT_FOUND;
        else if (_.AppInfo.Package.Id.Architecture is not ProcessorArchitecture.X64) throw ERROR_INSTALL_WRONG_PROCESSOR_ARCHITECTURE;

        return new() { AppDiagnosticInfo = _ };
    }

    public bool Running
    {
        get
        {
            var _ = AppDiagnosticInfo.GetResourceGroups().FirstOrDefault()?.GetProcessDiagnosticInfos().FirstOrDefault();
            return _ is not null && CloseHandle(OpenProcess(PROCESS_ALL_ACCESS, false, (int)_.ProcessId));
        }
    }

    public Package Package => AppDiagnosticInfo.AppInfo.Package;

    public Process Launch()
    {
        Marshal.ThrowExceptionForHR(PackageDebugSettings.EnableDebugging(Package.Id.FullName, default, default));
        Marshal.ThrowExceptionForHR(ApplicationActivationManager.ActivateApplication(AppDiagnosticInfo.AppInfo.AppUserModelId, default, AO_NOERRORUI, out var processId));
        return Process.GetProcessById(processId);
    }

    public void Terminate() => Marshal.ThrowExceptionForHR(PackageDebugSettings.TerminateAllProcesses(Package.Id.FullName));
}