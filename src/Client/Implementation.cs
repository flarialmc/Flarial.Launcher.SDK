using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Bedrockix.Minecraft;
using System.Threading;

namespace Flarial.Launcher.SDK;

readonly ref struct Build
{
    internal Build(string path, string address)
    {
        Path = path;
        Address = address;
    }

    internal readonly string Path;

    internal readonly string Address;
}

public static partial class Client
{
    static readonly HashAlgorithm Algorithm = SHA256.Create();

    static readonly object Lock = new();

    static readonly (string Uri, string Path, string Mutex) Release = new()
    {
        Uri = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll",
        Path = @"Flarial.Launcher.SDK\Flarial.Client.Release.dll",
        Mutex = "Flarial.Client.Release"
    };

    static readonly (string Uri, string Path, string Mutex) Beta = new()
    {
        Uri = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll",
        Path = @"Flarial.Launcher.SDK\Flarial.Client.Beta.dll",
        Mutex = "Flarial.Client.Beta"
    };

    static async Task<bool> VerifyAsync(string path, bool value = false) => await Task.Run(async () =>
    {
        if (!File.Exists(path)) return false;
        using var stream = File.OpenRead(path); var hash = await Web.HashAsync(value);
        lock (Lock) return hash.Equals(BitConverter.ToString(Algorithm.ComputeHash(stream)).Replace("-", string.Empty), StringComparison.OrdinalIgnoreCase);
    });

    static bool Loaded(string path)
    {
        path = Path.GetFullPath(path);
        return Metadata.Processes.Any(process =>
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

    public static async partial Task DownloadAsync(bool value, Action<int> action) => await Task.Run(async () =>
    {
        var (Uri, Path, _) = value ? Beta : Release;
        if (!await VerifyAsync(Path, value))
        {
            if (Loaded(Path)) Game.Terminate();
            Directory.CreateDirectory("Flarial.Launcher.SDK");
            await Web.DownloadAsync(Uri, Path, action);
        }
    });

    public static async partial Task<bool> LaunchAsync(bool value) => await Task.Run(() =>
    {
        if (Loaded((value ? Release : Beta).Path)) Game.Terminate();
        return Loader.Launch((value ? Beta : Release).Path).HasValue;
    });
}