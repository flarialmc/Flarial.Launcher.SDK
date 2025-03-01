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
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Operation;

    readonly TaskCompletionSource Source = new();

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operation, Action<int> action = default)
    {
        (Operation = operation).Completed += (sender, _) =>
        {
            if (sender.Status is AsyncStatus.Error) Source.TrySetException(sender.ErrorCode);
            else Source.TrySetResult();
        };
        if (action != default) Operation.Progress += (_, value) => { if (value.state is DeploymentProgressState.Processing) action((int)value.percentage); };
    }

    /// <summary>
    ///  Gets an awaiter for the installation request.
    /// </summary>
  
    public TaskAwaiter GetAwaiter() => Source.Task.GetAwaiter();

    /// <summary>
    /// Cancels the installation request.
    /// </summary>
  
    public void Cancel() { if (!Source.Task.IsCompleted) { Operation.Cancel(); ((IAsyncResult)Source.Task).AsyncWaitHandle.WaitOne(); } }

    /// <summary>
    ///  Asynchronously cancels the installation request.
    /// </summary>
   
    public async Task CancelAsync() { if (!Source.Task.IsCompleted) { Operation.Cancel(); await this; } }

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>
  
    public void Dispose() { Operation.Close(); Source.Task.Dispose(); GC.SuppressFinalize(this); }

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>
   
    ~Request() => Dispose();
}