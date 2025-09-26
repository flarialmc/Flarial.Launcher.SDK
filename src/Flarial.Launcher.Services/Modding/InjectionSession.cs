using System;
using Windows.Win32.Foundation;
using System.Collections.Generic;
using Flarial.Launcher.Services.System;

namespace Flarial.Launcher.Services.Modding;

sealed class InjectionSession : IDisposable
{
    readonly RemoteThreadHandle _threadHandle;

    readonly HANDLE _processHandle;

    readonly List<nint> _addresses;

    internal static InjectionSession Create(ProcessHandle processHandle) => new(processHandle);

    internal void Add() {}

    internal void Inject() {}

    InjectionSession(ProcessHandle processHandle)
    {
        _addresses = [];
        _processHandle = processHandle;
        _threadHandle = RemoteThreadHandle.Create(processHandle);
    }

    public void Dispose()
    {
        _threadHandle.Dispose();
    }
}