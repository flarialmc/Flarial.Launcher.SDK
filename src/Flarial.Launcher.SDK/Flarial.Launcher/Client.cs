namespace Flarial.Launcher;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Minecraft.UWP;

/// <summary>
/// Provides method to interact with Flarial Client's dynamic link library.
/// </summary>
public static class Client
{
    static readonly int Size = Environment.SystemPageSize;

    internal static async Task GetAsync(this HttpClient source, string requestUri, string path, Action<int> action = default)
    {
        using var message = await source.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
        message.EnsureSuccessStatusCode();

        using var stream = await message.Content.ReadAsStreamAsync();
        using var destination = File.OpenWrite(path);

        var count = 0;
        var value = 0L;
        var buffer = new byte[Size];

        while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) is not 0)
        {
            await destination.WriteAsync(buffer, 0, count);
            if (action is not null) action((int)Math.Round(100F * (value += count) / message.Content.Headers.ContentLength.Value));
        }
    }

    static readonly (string, string) Release = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll", @"Client\Flarial.Client.dll");

    static readonly (string, string) Beta = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll", @"Client\Flarial.Client.Beta.dll");

    static Client() => Directory.CreateDirectory("Client");

    /// <summary>
    /// Asynchronously download Flarial Client's dynamic link library.
    /// </summary>
    /// <param name="_">Specify <c>true</c> to download Flarial Client's Beta.</param>
    /// <param name="action">Callback for download progress.</param>
    /// <returns></returns>
    public static async Task DownloadAsync(bool _ = false, Action<int> action = default)
    {
        var (requestUri, path) = _ ? Beta : Release;
        await Global.HttpClient.GetAsync(requestUri, path, action);
    }

    /// <summary>
    /// Asynchronously launch Minecraft &#38; inject Flarial Client's dynamic link library.
    /// </summary>
    /// <param name="_">Specify <c>true</c> to use Flarial Client's Beta.</param>
    /// <returns></returns>
    public static async Task ActivateAsync(bool _ = false) => await Injector.InjectAsync((_ ? Beta : Release).Item2);
}