namespace Minecraft.UWP;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IPackageDebugSettings
{
    [PreserveSig]
    int EnableDebugging(string packageFullName, string debuggerCommandLine, string environment);

    [PreserveSig]
    int DisableDebugging(string packageFullName);

    [PreserveSig]
    int Suspend(string packageFullName);

    [PreserveSig]
    int Resume(string packageFullName);

    [PreserveSig]
    int TerminateAllProcesses(string packageFullName);

    [PreserveSig]
    int SetTargetSessionId(ulong sessionId);

    [PreserveSig]
    int EnumerateBackgroundTasks(string packageFullName, nint taskCount, nint taskIds, nint taskNames);

    [PreserveSig]
    int ActivateBackgroundTask(nint taskId);

    [PreserveSig]
    int StartServicing(string packageFullName);

    [PreserveSig]
    int StopServicing(string packageFullName);

    int StartSessionRedirection(string packageFullName, ulong sessionId);

    [PreserveSig]
    int StopSessionRedirection(string packageFullName);

    [PreserveSig]
    int GetPackageExecutionState(string packageFullName, nint packageExecutionState);

    [PreserveSig]
    int RegisterForPackageStateChanges(string packageFullName, nint pPackageExecutionStateChangeNotification, nint pdwCookie);

    [PreserveSig]
    int UnregisterForPackageStateChanges(uint dwCookie);
}

[ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
sealed class PackageDebugSettings : IPackageDebugSettings
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int EnableDebugging(string packageFullName, string debuggerCommandLine, string environment);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int DisableDebugging(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int Suspend(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int Resume(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int TerminateAllProcesses(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int SetTargetSessionId(ulong sessionId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int EnumerateBackgroundTasks(string packageFullName, nint taskCount, nint taskIds, nint taskNames);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int ActivateBackgroundTask(nint taskId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int StartServicing(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int StopServicing(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int StartSessionRedirection(string packageFullName, ulong sessionId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int StopSessionRedirection(string packageFullName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetPackageExecutionState(string packageFullName, nint packageExecutionState);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int RegisterForPackageStateChanges(string packageFullName, nint pPackageExecutionStateChangeNotification, nint pdwCookie);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern int UnregisterForPackageStateChanges(uint dwCookie);
}