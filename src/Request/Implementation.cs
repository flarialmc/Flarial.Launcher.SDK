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
            try
            {
                if (sender.Status is AsyncStatus.Error)
                    Completion.TrySetException(sender.ErrorCode);
                else
                    Completion.TrySetResult(default);
            }
            finally { Cancellation.TrySetResult(default); }
        };

        if (action != default)
            Operation.Progress += (_, value) =>
            {
                if (value.state is DeploymentProgressState.Processing)
                    action((int)value.percentage);
            };
    }

    public partial ConfiguredTaskAwaitable<object>.ConfiguredTaskAwaiter GetAwaiter() => Completion.Task.ConfigureAwait(false).GetAwaiter();

    public partial void Cancel()
    {
        if (!Cancellation.Task.IsCompleted)
        {
            Operation.Cancel();
            Cancellation.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public partial async Task CancelAsync()
    {
        if (!Cancellation.Task.IsCompleted)
        {
            Operation.Cancel();
            await Cancellation.Task.ConfigureAwait(false);
        }
    }
}