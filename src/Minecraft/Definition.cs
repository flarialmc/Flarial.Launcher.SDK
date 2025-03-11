using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Provides methods to interact with Minecraft: Bedrock Edition.
/// </summary>

[Obsolete]
public static partial class Minecraft
{
    /// <summary>
    /// Check if Minecraft: Bedrock Edition is installed.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Game.Install`.")]
    public static partial bool Installed { get; }

    /// <summary>
    /// Check if Minecraft Bedrock is running.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Game.Running`.")]
    public static partial bool Running { get; }

    /// <summary>
    /// Configure debug mode for Minecraft: Bedrock Edition.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Game.Debug`.")]
    public static partial bool Debug { set; }

    /// <summary>
    /// Launches Minecraft Bedrock Edition.
    /// </summary>

    /// <returns>
    /// If the game initialized &amp; launched successfully or not.
    /// </returns>

    [Obsolete("Use `Bedrockix.Minecraft.Game.Launch()`.")]
    public static partial bool Launch();

    /// <summary>
    /// Launches &amp; loads a dynamic link library into Minecraft: Bedrock Edition.
    /// </summary>

    /// <param name="path">
    /// The dynamic link library to load.
    /// </param>

    /// <returns>
    /// If the game initialized &amp; launched successfully or not.
    /// </returns>

    [Obsolete("Use `Bedrockix.Minecraft.Loader.Launch(string)`.")]
    public static partial bool Launch(string path);

    /// <summary>
    /// Terminates Minecraft: Bedrock Edition.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Game.Terminate()`.")]
    public static partial void Terminate();

    /// <summary>
    /// Get Minecraft: Bedrock Edition's version.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Metadata.Version`.")]
    public static partial string Version { get; }

    /// <summary>
    /// Get any running processes of Minecraft: Bedrock Edition.
    /// </summary>

    [Obsolete("Use `Bedrockix.Minecraft.Metadata.Processes`.")]
    public static partial IEnumerable<Process> Processes { get; }

    /// <summary>
    /// Asynchronously launches &amp; loads a dynamic link library into Minecraft: Bedrock Edition.
    /// </summary>

    /// <param name="path">
    /// The dynamic link library to load.
    /// </param>

    /// <returns>
    /// If the game initialized &amp; launched successfully or not.
    /// </returns>

    [Obsolete("Use `await Task.Run(() => Bedrockix.Minecraft.Loader.Launch(string).HasValue)`.")]
    public static partial Task<bool> LaunchAsync(string path);
}