using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Provides methods to interact with Flarial Client's dynamic link library.
/// </summary>

public static partial class Client
{
    static readonly HashAlgorithm Algorithm = SHA256.Create();

    static readonly object Lock = new();

    static readonly (string RequestUri, string Path) Release = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll", @"Flarial.Launcher.SDK\Flarial.Client.Release.dll");

    static readonly (string RequestUri, string Path) Beta = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll", @"Flarial.Launcher.SDK\Flarial.Client.Beta.dll");

    static async Task<bool> VerifyAsync(string path, bool value = false) => await Task.Run(async () =>
    {
        if (!File.Exists(path)) return false;
        using var stream = File.OpenRead(path); var hash = await Web.HashAsync(value);
        lock (Lock) return hash.Equals(BitConverter.ToString(Algorithm.ComputeHash(stream)).Replace("-", string.Empty), StringComparison.OrdinalIgnoreCase);
    });

    static bool Loaded(string path)
    {
        path = Path.GetFullPath(path);
        return Minecraft.Processes.Any(process =>
        {
            using (process)
            {
                foreach (ProcessModule module in process.Modules)
                    using (module)
                        if (path.Equals(module.FileName, StringComparison.OrdinalIgnoreCase))
                            return true;
                return false;
            }
        });
    }

    /// <summary>
    /// Asynchronously download Flarial Client's dynamic link library.
    /// </summary>

    /// <param name="value">
    /// Specify <c>true</c> to download Flarial Client's Beta.
    /// </param>

    /// <param name="action">
    /// Callback for download progress.
    /// </param>

    public static async Task DownloadAsync(bool value = false, Action<int> action = default) => await Task.Run(async () =>
    {
        var (Uri, Path) = value ? Beta : Release;
        if (!await VerifyAsync(Path, value))
        {
            if (Loaded(Path)) Minecraft.Terminate();
            Directory.CreateDirectory("Flarial.Launcher.SDK");
            await Web.DownloadAsync(Uri, Path, action);
        }
    });

    /// <summary>
    /// Asynchronously launch Minecraft &#38; load Flarial Client's dynamic link library.
    /// </summary>

    /// <param name="value">
    /// Specify <c>true</c> to use Flarial Client's Beta.
    /// </param>

    /// <returns>
    /// If the game initialized &amp; launched successfully or not.
    /// </returns>

    public static async Task<bool> LaunchAsync(bool value = false) => await Task.Run(() =>
    {
        if (Loaded((value ? Release : Beta).Path)) Minecraft.Terminate();
        return Minecraft.Launch((value ? Beta : Release).Path);
    });
}