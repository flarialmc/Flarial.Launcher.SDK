namespace Flarial.Launcher;

using System;
using System.IO;
using System.Threading.Tasks;
using Minecraft.UWP;

/// <summary>
/// Provides method to interact with Flarial Client's dynamic link library.
/// </summary>
public static class Client
{
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