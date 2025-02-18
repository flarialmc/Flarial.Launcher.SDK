using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bedrockix.Minecraft;

namespace Flarial.Launcher;

public static class Minecraft
{
    public static bool Installed => Game.Installed;

    public static bool Debug { set { Game.Debug = value; } }

    public static bool Running => Game.Running;

    public static int Launch() => Game.Launch();

    public static void Launch(string path) => Loader.Launch(path);

    public static void Terminate() => Game.Terminate();

    public static async Task<int> LaunchAsync() => await Task.Run(Game.Launch);

    public static async Task LaunchAsync(string path) => await Task.Run(() => Loader.Launch(path));

    public static async Task TerminateAsync() => await Task.Run(Game.Terminate);
}