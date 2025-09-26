using System.IO;
using Flarial.Launcher.Services.System;
using Windows.Management.Core;
using Windows.Win32.Foundation;
using Windows.Win32.Globalization;
using static Windows.Win32.PInvoke;

namespace Flarial.Launcher.Services.Core;

unsafe sealed class MinecraftUWP : Minecraft
{
    const string PackageFamilyName = "Microsoft.MinecraftUWP_8wekyb3d8bbwe";

    const string ApplicationModelUserId = $"{PackageFamilyName}!App";

    internal MinecraftUWP() : base(PackageFamilyName, ApplicationModelUserId) { }

    public override bool IsRunning
    {
        get
        {
            fixed (char* @class = "MSCTFIME UI")
            fixed (char* string1 = _applicationModelUserId)
            {
                HWND window = HWND.Null;
                var length = APPLICATION_USER_MODEL_ID_MAX_LENGTH;
                var string2 = stackalloc char[(int)length];

                while ((window = FindWindowEx(HWND.Null, window, @class, null)) != HWND.Null)
                {
                    uint processId = 0;
                    GetWindowThreadProcessId(window, &processId);

                    if (ProcessHandle.Open(processId) is not { } process)
                        continue;

                    using (process)
                    {
                        var error = GetApplicationUserModelId(process, &length, string2);
                        if (error is not WIN32_ERROR.ERROR_SUCCESS) continue;

                        var result = CompareStringOrdinal(string1, -1, string2, -1, true);
                        if (result is not COMPARESTRING_RESULT.CSTR_EQUAL) continue;

                        return true;
                    }
                }

            }
            return false;
        }
    }

    internal override ProcessHandle? LaunchProcess()
    {
        if (IsRunning)
            return ProcessHandle.Open(Activate());

        var path = ApplicationDataManager.CreateForPackageFamily(_packageFamilyName).LocalFolder.Path;
        path = Path.Combine(path, @"games\com.mojang\minecraftpe\resource_init_lock");

        fixed (char* @string = path)
        {
            FileHandle? file = null;
            using var process = ProcessHandle.Open(Activate());

            try
            {
                while (process?.IsRunning(1) ?? false)
                {
                    file ??= FileHandle.Open(@string);
                    if (file?.IsDeleted ?? false) return process;
                }
            }
            finally { file?.Dispose(); }
        }

        return null;
    }

    public override void Terminate()
    {
        uint count = 1U, length = PACKAGE_FULL_NAME_MAX_LENGTH;
        PWSTR packageFullNames = new(), packageFullName = stackalloc char[(int)length];

        GetPackagesByPackageFamily(_packageFamilyName, ref count, &packageFullNames, ref length, packageFullName);
        _packageDebugSettings.TerminateAllProcesses(packageFullName);
    }
}