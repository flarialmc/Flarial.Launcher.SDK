namespace Launcher;

using System;
using System.IO;
using System.Threading.Tasks;
using Minecraft;

static class Client
{
    static readonly (string Url, string Path) Release = new()
    {
        Url = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll",
        Path = @"Client\Latest.dll"
    };

    static readonly (string Url, string Path) Beta = new()
    {
        Url = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll",
        Path = @"Client\Beta.dll"
    };

    static Client() => Directory.CreateDirectory("Client");

    internal static async Task DownloadAsync(Action<int> action, bool _ = false)
    {
        await Task.Run(Game.Terminate);
        await Internet.DownloadAsync((_ ? Beta : Release).Url, (_ ? Beta : Release).Path, action);
    }

    internal static async Task LaunchAsync(bool _ = false) => await Task.Run(() => Injector.Inject(Game.Launch(), (_ ? Beta : Release).Path));
}