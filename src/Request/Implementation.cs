using System;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using System.Runtime.CompilerServices;

namespace Flarial.Launcher.SDK;

public sealed partial class Request : IDisposable
{
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Operation;

    readonly TaskCompletionSource<object> Source = new();

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operation, Action<int> action = default)
    {
        (Operation = operation).Completed += (sender, _) =>
        {
            if (sender.Status is AsyncStatus.Error) Source.TrySetException(sender.ErrorCode);
            else Source.TrySetResult(default);
        };
        if (action != default) Operation.Progress += (_, value) => { if (value.state is DeploymentProgressState.Processing) action((int)value.percentage); };
    }

    public partial TaskAwaiter<object> GetAwaiter() => Source.Task.GetAwaiter();

    public partial void Cancel()
    {
        if (!Source.Task.IsCompleted)
        {
            Operation.Cancel();
            ((IAsyncResult)Source.Task).AsyncWaitHandle.WaitOne();
        }
    }

    public partial async Task CancelAsync()
    {
        if (!Source.Task.IsCompleted)
        {
            Operation.Cancel();
            await this;
        }
    }

    public partial void Dispose() { Operation.Close(); Source.Task.Dispose(); GC.SuppressFinalize(this); }

}