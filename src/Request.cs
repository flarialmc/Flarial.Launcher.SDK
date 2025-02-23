using System;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using System.Runtime.CompilerServices;

namespace Flarial.Launcher;

/// <summary>
/// Represents an installation request for a version.
/// </summary>
public sealed class Request
{
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Value;

    readonly TaskCompletionSource Object = new();

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> value, Action<int> action = default)
    {
        (Value = value).Completed += (_, _) => Object.SetResult();
        if (action != default) Value.Progress += (_, @object) => { if (@object.state is DeploymentProgressState.Processing) action((int)@object.percentage); };
    }

    public TaskAwaiter GetAwaiter() => Object.Task.GetAwaiter();

    /// <summary>
    /// Cancels the installation request.
    /// </summary>
    public void Cancel() { Value.Cancel(); ((IAsyncResult)Object.Task).AsyncWaitHandle.WaitOne(); }

    /// <summary>
    ///  Asynchronously cancels the installation request.
    /// </summary>
    public async Task CancelAsync() { Value.Cancel(); await this; }
}