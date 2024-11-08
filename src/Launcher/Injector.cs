namespace Launcher;

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using static Native;

static class Injector
{
    static readonly nint lpStartAddress;

    static readonly SecurityIdentifier Identifier = new("S-1-15-2-1");

    static Injector()
    {
        nint hModule = default;
        try { hModule = LoadLibraryEx("Kernel32.dll", default, LOAD_LIBRARY_SEARCH_SYSTEM32); lpStartAddress = GetProcAddress(hModule, "LoadLibraryW"); }
        finally { FreeLibrary(hModule); }
    }

    internal static void Inject(int processId, string path)
    {
        Logger.Log("Attempting to inject specified dynamic link library.");

        FileInfo info = new(path = Path.GetFullPath(path)); var security = info.GetAccessControl();
        security.AddAccessRule(new(Identifier, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
        info.SetAccessControl(security);

        nint hProcess = default, lpBaseAddress = default, hThread = default;
        try
        {
            hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            if (hProcess == default) throw new Win32Exception(Marshal.GetLastWin32Error());

            var size = sizeof(char) * (path.Length + 1);

            lpBaseAddress = VirtualAllocEx(hProcess, default, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
            if (lpBaseAddress == default) throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!WriteProcessMemory(hProcess, lpBaseAddress, Marshal.StringToHGlobalUni(path), size)) throw new Win32Exception(Marshal.GetLastWin32Error());

            hThread = CreateRemoteThread(hProcess, default, 0, lpStartAddress, lpBaseAddress, 0);
            if (hThread == default) throw new Win32Exception(Marshal.GetLastWin32Error());
            _ = WaitForSingleObject(hThread, Timeout.Infinite);
        }
        finally
        {
            VirtualFreeEx(hProcess, lpBaseAddress, 0, MEM_RELEASE);
            CloseHandle(hThread);
            CloseHandle(hProcess);
        }

        Logger.Log("The specified dynamic link library has been injected successfully.");
    }
}