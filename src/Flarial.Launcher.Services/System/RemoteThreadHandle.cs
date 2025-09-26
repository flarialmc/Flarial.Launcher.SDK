using System;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;
using static Windows.Win32.PInvoke;
using static Windows.Win32.System.Threading.PROCESS_CREATION_FLAGS;

namespace Flarial.Launcher.Services.System;

unsafe sealed class RemoteThreadHandle : IDisposable
{
    readonly HANDLE _threadHandle;

    readonly static LPTHREAD_START_ROUTINE _address;

    static RemoteThreadHandle()
    {
        var module = GetModuleHandle("Kernel32");
        var procedure = GetProcAddress(module, "LoadLibraryW");
        _address = procedure.CreateDelegate<LPTHREAD_START_ROUTINE>();
    }

    internal static RemoteThreadHandle Create(in ProcessHandle processHandle)
    {
        var threadHandle = CreateRemoteThread(processHandle, null, 0, _address, null, (uint)CREATE_SUSPENDED, null);
        return new(threadHandle);
    }

    RemoteThreadHandle(HANDLE threadHandle)
    {
        _threadHandle = threadHandle;
    }

    public void Resume()
    {
        ResumeThread(_threadHandle);
        WaitForSingleObject(_threadHandle, INFINITE);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        TerminateThread(_threadHandle, 0);
        CloseHandle(_threadHandle);
    }

    ~RemoteThreadHandle() => Dispose();
}