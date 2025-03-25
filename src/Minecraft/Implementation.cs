using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Bedrockix.Minecraft;

namespace Flarial.Launcher.SDK;

public static partial class Minecraft
{
    public static partial bool Installed => Game.Installed;

    public static partial bool Running => Game.Running;

    public static partial bool Debug { set { Game.Debug = value; } }

    public static partial bool Launch() => Game.Launch().HasValue;

    public static partial bool Launch(string path) => Loader.Launch(path).HasValue;

    public static partial void Terminate() => Game.Terminate();

    public static partial string Version => Metadata.Version;

    public static partial IEnumerable<Process> Processes => Metadata.Processes;

    public static async partial Task<bool> LaunchAsync(string path) => await Task.Run(() => Launch(path));
}