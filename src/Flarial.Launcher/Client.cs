namespace Flarial.Launcher;

using System;
using System.IO;
using System.Linq;
using Minecraft.UWP;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

/// <summary>
/// Provides method to interact with Flarial Client's dynamic link library.
/// </summary>
public static class Client
{
    static Client() => Directory.CreateDirectory("Client");

    static readonly int Size = Environment.SystemPageSize;

    static readonly HashAlgorithm Algorithm = SHA256.Create();

    static readonly object Object = new();

    static readonly (string RequestUri, string Path) Release = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll", @"Client\Flarial.Client.Release.dll");

    static readonly (string RequestUri, string Path) Beta = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll", @"Client\Flarial.Client.Beta.dll");

    const string Hashes = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll_hashes.json";

    static async Task<bool> Verify(string path, bool value = false)
    {
        if (!File.Exists(path)) return false;
        using var stream = File.OpenRead(path);
        var hash = Json.Parse(await Global.HttpClient.GetStreamAsync(Hashes))[value ? "Beta" : "Release"].Value;
        lock (Object) return hash.Equals(BitConverter.ToString(Algorithm.ComputeHash(stream)).Replace("-", string.Empty), StringComparison.OrdinalIgnoreCase);
    }

    static async Task GetAsync(this HttpClient source, string requestUri, string path, Action<int> action = default)
    {
        using var message = await source.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
        message.EnsureSuccessStatusCode();

        using var stream = await message.Content.ReadAsStreamAsync();
        using var destination = File.OpenWrite(path);

        var count = 0; var value = 0L; var buffer = new byte[Size];
        while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) is not 0)
        {
            await destination.WriteAsync(buffer, 0, count);
            if (action is not null) action((int)Math.Round(100F * (value += count) / message.Content.Headers.ContentLength.Value));
        }
    }

    static bool Loaded(string path)
    {
        path = Path.GetFullPath(path);
        return Game.Processes.Any(process =>
        {
            using (process)
            {
                foreach (ProcessModule module in process.Modules) using (module) if (path.Equals(module.FileName, StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }
        });
    }

    /// <summary>
    /// Asynchronously download Flarial Client's dynamic link library.
    /// </summary>
    /// <param name="value">Specify <c>true</c> to download Flarial Client's Beta.</param>
    /// <param name="action">Callback for download progress.</param>
    public static async Task DownloadAsync(bool value = false, Action<int> action = default) => await Task.Run(async () =>
    {
        var (requestUri, path) = value ? Beta : Release;
        if (!await Verify(path, value))
        {
            if (Loaded(path)) Game.Terminate();
            await Global.HttpClient.GetAsync(requestUri, path, action);
        }
    });

    /// <summary>
    /// Asynchronously launch Minecraft &#38; inject Flarial Client's dynamic link library.
    /// </summary>
    /// <param name="value">Specify <c>true</c> to use Flarial Client's Beta.</param>
    public static async Task LaunchAsync(bool value = false) => await Task.Run(() =>
    {
        if (Loaded((value ? Release : Beta).Path)) Game.Terminate();
        Injector.Inject(Game.Launch(), (value ? Beta : Release).Path);
    });
}