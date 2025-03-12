using System;
using System.Security;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Provides methods to manage Flarial Client's launcher.
/// </summary>

[SuppressUnmanagedCodeSecurity]
public static partial class Launcher
{
    /// <summary>
    /// Asynchronously check if a launcher update is available. 
    /// </summary>

    public static partial Task<bool> AvailableAsync();

    /// <summary>
    ///  Asynchronously force updates the launcher to the latest version.
    /// </summary>
    /// <param name="action">Callback for update progress.</param>

    public static partial Task UpdateAsync(Action<int> action = default);
}