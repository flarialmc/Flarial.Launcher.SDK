using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;
using static Windows.Win32.Foundation.WIN32_ERROR;
using Windows.Win32.UI.Shell;
using Flarial.Launcher.Services.System;

namespace Flarial.Launcher.Services.Core;

public unsafe abstract class Minecraft
{
    internal Minecraft(string packageFamilyName, string applicationModelUserId)
    {
        _packageFamilyName = packageFamilyName;
        _applicationModelUserId = applicationModelUserId;
    }

    static Minecraft()
    {
        ApplicationActivationManager applicationActivationManager = new();
        PackageDebugSettings packageDebugSettings = new();

        _applicationActivationManager = (IApplicationActivationManager)applicationActivationManager;
        _packageDebugSettings = (IPackageDebugSettings)packageDebugSettings;
    }

    static readonly IApplicationActivationManager _applicationActivationManager;

    static readonly IPackageDebugSettings _packageDebugSettings;

    protected readonly string _packageFamilyName, _applicationModelUserId;

    public bool IsInstalled
    {
        get
        {
            uint count = 0U, length = 0U;
            PWSTR packageFullNames = new();

            var error = GetPackagesByPackageFamily(_packageFamilyName, ref count, &packageFullNames, ref length, null);
            return error is ERROR_INSUFFICIENT_BUFFER && count > 0;
        }
    }

    public bool HasUWPAppLifecycle
    {
        set
        {
            var count = 1U;
            var length = PACKAGE_FULL_NAME_MAX_LENGTH;

            PWSTR packageFullNames = new();
            PWSTR packageFullName = stackalloc char[(int)length];
            GetPackagesByPackageFamily(_packageFamilyName, ref count, &packageFullNames, ref length, packageFullName);

            if (value) _packageDebugSettings.EnableDebugging(packageFullName, null, null);
            else _packageDebugSettings.DisableDebugging(packageFullName);

        }
    }

    protected uint Activate()
    {
        fixed (char* applicationModelUserId = _applicationModelUserId)
        {
            _applicationActivationManager.ActivateApplication(applicationModelUserId, null, ACTIVATEOPTIONS.AO_NOERRORUI, out var processId);
            return processId;
        }
    }

    public uint? Launch()
    {
        using var process = LaunchProcess();
        return process?.ProcessId;
    }

    public abstract bool IsRunning { get; }

    internal abstract ProcessHandle? LaunchProcess();

    public abstract void Terminate();
}