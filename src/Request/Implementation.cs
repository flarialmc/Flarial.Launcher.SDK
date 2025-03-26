using System;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using System.Runtime.CompilerServices;

namespace Flarial.Launcher.SDK;

public sealed partial class Request
{
    readonly IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> Operation;

    readonly TaskCompletionSource<object> Completion = new(), Cancellation = new();

    internal Request(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> operation, Action<int> action = default)
    {
        (Operation = operation).Completed += (sender, _) =>
        {
            if (sender.Status is AsyncStatus.Error)
                Completion.TrySetException(sender.ErrorCode);
            else
                Completion.TrySetResult(default);
            Cancellation.TrySetResult(default);
        };

        if (action != default)
            Operation.Progress += (_, value) =>
            {
                if (value.state is DeploymentProgressState.Processing)
                    action((int)value.percentage);
            };
    }

    public partial TaskAwaiter<object> GetAwaiter() => Completion.Task.GetAwaiter();

    public partial void Cancel()
    {
        if (Cancellation.Task.IsCompleted) return;
        Operation.Cancel();
        Cancellation.Task.GetAwaiter().GetResult();
    }

    public partial async Task CancelAsync()
    {
        if (Cancellation.Task.IsCompleted) return;
        Operation.Cancel();
        await Cancellation.Task;
    }
}