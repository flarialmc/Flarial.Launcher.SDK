using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

namespace Flarial.Launcher.SDK;

public static partial class Developer
{
    const int S_OK = default;

    [DllImport("WSClient.dll"), PreserveSig]
    static extern int CheckDeveloperLicense(out nint pExpiration);

    [DllImport("WSClient.dll")]
    static extern void RemoveDeveloperLicense(nint hwndParent);

    public static partial bool Enabled => CheckDeveloperLicense(out _) is S_OK;

    public static partial void Request() => RemoveDeveloperLicense(default);
}