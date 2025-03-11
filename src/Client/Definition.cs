using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Flarial.Launcher.SDK;


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

    public static async partial Task DownloadAsync(bool value, Action<int> action) => await Task.Run(async () =>
    {
        var (Uri, Path) = value ? Beta : Release;
        if (!await VerifyAsync(Path, value))
        {
            if (Loaded(Path)) Minecraft.Terminate();
            Directory.CreateDirectory("Flarial.Launcher.SDK");
            await Web.DownloadAsync(Uri, Path, action);
        }
    });

    public static async partial Task<bool> LaunchAsync(bool value) => await Task.Run(() =>
    {
        if (Loaded((value ? Release : Beta).Path)) Minecraft.Terminate();
        return Minecraft.Launch((value ? Beta : Release).Path);
    });
}