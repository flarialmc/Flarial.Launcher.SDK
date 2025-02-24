using System;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using System.Runtime.CompilerServices;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Represents an installation request for a version.
/// </summary>
public sealed class Request : IDisposable
{
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Value;

    readonly TaskCompletionSource Object = new();

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> value, Action<int> action = default)
    {
        (Value = value).Completed += (_, _) => Object.TrySetResult();
        if (action != default) Value.Progress += (_, @object) => { if (@object.state is DeploymentProgressState.Processing) action((int)@object.percentage); };
    }

    /// <summary>
    /// Asynchronously wait for the installation request to complete.
    /// </summary>
    public TaskAwaiter GetAwaiter() => Object.Task.GetAwaiter();

    /// <summary>
    /// Cancels the installation request.
    /// </summary>
    public void Cancel() { if (!Object.Task.IsCompleted) { Value.Cancel(); ((IAsyncResult)Object.Task).AsyncWaitHandle.WaitOne(); } }

    /// <summary>
    ///  Asynchronously cancels the installation request.
    /// </summary>
    public async Task CancelAsync() { if (!Object.Task.IsCompleted) { Value.Cancel(); await this; } }

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>
    public void Dispose() { Value.Close(); Object.Task.Dispose(); GC.SuppressFinalize(this); }

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>
    ~Request() => Dispose();
}