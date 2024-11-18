using System;
using System.Net.Http;
using System.Threading;
using Windows.Management.Deployment;

static class Global
{
    internal static readonly HttpClient HttpClient = new();

    internal static readonly PackageManager PackageManager = new();
}

readonly struct _ : IDisposable
{
    readonly SynchronizationContext SyncContext;

    public _()
    {
        SyncContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
    }

    public void Dispose() => SynchronizationContext.SetSynchronizationContext(SyncContext);
}