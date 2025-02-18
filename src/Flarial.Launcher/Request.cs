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
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Operation;

    readonly Task Task;

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> value, Action<int> action = default)
    {
        Operation = value;
        Task = action is null ? value.AsTask() : value.AsTask(new Progress<DeploymentProgress>(_ =>
        {
            if (_.state is DeploymentProgressState.Processing)
                action((int)_.percentage);
        }));
    }

    public TaskAwaiter GetAwaiter() => Task.GetAwaiter();

    /// <summary>
    /// Cancels the installation request.
    /// </summary>
    public void Cancel()
    {
        Operation.Cancel();
        ((IAsyncResult)Task).AsyncWaitHandle.WaitOne();
    }

    /// <summary>
    ///  Asynchronously cancels the installation request.
    /// </summary>
    public async Task CancelAsync() => await Task.Run(Cancel);
}