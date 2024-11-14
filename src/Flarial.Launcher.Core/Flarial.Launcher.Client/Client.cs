namespace Flarial.Launcher.Client;

using System;
using System.IO;
using System.Threading.Tasks;
using Minecraft.UWP;

public static class Client
{
    static readonly (string, string) Release = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/latest.dll", @"Client\Flarial.Client.dll");

    static readonly (string, string) Beta = new("https://raw.githubusercontent.com/flarialmc/newcdn/main/dll/beta.dll", @"Client\Flarial.Client.Beta.dll");

    static Client() => Directory.CreateDirectory("Client");

    public static async Task DownloadAsync( bool _ = false,Action<int> action = default)
    {
        using (new _())
        {
            var (requestUri, path) = _ ? Beta : Release;
            await Global.HttpClient.GetAsync(requestUri, path, action);
        }
    }

    public static async Task ActivateAsync(bool _ = false) { using (new _()) await ActivateAsync((_ ? Beta : Release).Item2); }

    public static async Task ActivateAsync(string path) { using (new _()) await Task.Run(() => Injector.Inject(Game.Activate(), path)); }
}