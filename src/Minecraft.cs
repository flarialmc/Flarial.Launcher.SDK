using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Bedrockix.Minecraft;

namespace Flarial.Launcher;

/// <summary>
/// Provides methods to interact with Minecraft: Bedrock Edition.
/// </summary>
public static class Minecraft
{
    /// <summary>
    /// Check if Minecraft: Bedrock Edition is installed.
    /// </summary>
    public static bool Installed => Game.Installed;

    /// <summary>
    /// Check if Minecraft Bedrock is running.
    /// </summary>
    public static bool Running => Game.Running;

    /// <summary>
    /// Configure debug mode for Minecraft: Bedrock Edition.
    /// </summary>
    public static bool Debug { set { Game.Debug = value; } }

    /// <summary>
    /// Launches Minecraft Bedrock Edition.
    /// </summary>
    public static void Launch() => Game.Launch();

    /// <summary>
    /// Launches &amp; loads a dynamic link library into Minecraft: Bedrock Edition.
    /// </summary>
    /// <param name="path">The dynamic link library to load.</param>
    public static void Launch(string path) => Loader.Launch(path);

    /// <summary>
    /// Terminates Minecraft: Bedrock Edition.
    /// </summary>
    public static void Terminate() => Game.Terminate();

    /// <summary>
    /// Get Minecraft: Bedrock Edition's version.
    /// </summary>
    public static string Version => Metadata.Version;

    /// <summary>
    /// Get any running processes of Minecraft: Bedrock Edition.
    /// </summary>
    public static IEnumerable<Process> Processes => Metadata.Processes;

    /// <summary>
    /// Asynchronously launches &amp; loads a dynamic link library into Minecraft: Bedrock Edition.
    /// </summary>
    /// <param name="path">The dynamic link library to load.</param>
    public static async Task LaunchAsync(string path) => await Task.Run(() => Launch(path));
}