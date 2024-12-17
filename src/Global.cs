using System;
using System.Net.Http;
using System.Threading;
using Windows.Foundation;

static class Global
{
    internal static readonly HttpClient HttpClient = new();

    internal const string PackageFamilyName = "Microsoft.MinecraftUWP_8wekyb3d8bbwe";

    internal const string AppUserModelId = "Microsoft.MinecraftUWP_8wekyb3d8bbwe!App";

    internal static T Get<T>(this IAsyncOperation<T> source)
    {
        using ManualResetEventSlim @event = new(false);
        source.Completed += (_, _) => @event.Set();
        if (source.Status is AsyncStatus.Started) @event.Wait();
        return source.Status switch
        {
            AsyncStatus.Completed => source.GetResults(),
            AsyncStatus.Error => throw source.ErrorCode,
            AsyncStatus.Canceled => throw new OperationCanceledException(),
            _ => throw new InvalidOperationException()
        };
    }
}