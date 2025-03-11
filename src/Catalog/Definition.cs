using System;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Provides methods to manage Minecraft versions compatible with Flarial Client.
/// </summary>

public sealed partial class Catalog
{
    /// <summary>
    /// Asynchronously gets a catalog of versions.
    /// </summary>

    /// <returns>
    /// A catalog of versions supported by Flarial Client.
    /// </returns>

    public static partial Task<Catalog> GetAsync();

    /// <summary>
    /// Checks if the installed version of Minecraft Bedrock Edition is compatible with Flarial.
    /// </summary>

    /// <returns>
    /// A boolean value that represents compatibility.
    /// </returns>

    public partial Task<bool> CompatibleAsync();

    /// <summary>
    /// Asynchronously starts the installation of a version.
    /// </summary>

    /// <param name="value">
    /// The version to be installed.
    /// </param>

    /// <param name="action">
    /// Callback for installation progress.
    /// </param>

    /// <returns>
    /// An installation request.
    /// </returns>

    public partial Task<Request> InstallAsync(string value, Action<int> action = default);
}