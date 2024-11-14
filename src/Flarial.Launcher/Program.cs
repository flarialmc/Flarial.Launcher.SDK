using System;
using System.IO;
using System.Threading.Tasks;
using Flarial.Launcher.Client;
using Minecraft.UWP;

static class Program
{
    static async Task Main()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Flarial.Launcher");
        Directory.CreateDirectory(path); Directory.SetCurrentDirectory(path);

        Console.WriteLine("Injecting Flarial Client: Release...");
        await Game.TerminateAsync(); await Client.DownloadAsync(); await Client.ActivateAsync();

        Console.WriteLine("Injecting Flarial Client: Beta...");
        await Game.TerminateAsync(); await Client.DownloadAsync(true); await Client.ActivateAsync(true);
    }
}