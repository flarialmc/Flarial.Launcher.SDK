using System;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using System.Runtime.CompilerServices;

namespace Flarial.Launcher.SDK;

/// <summary>
/// Represents an installation request for a version.
/// </summary>

public sealed partial class Request : IDisposable
{
    /// <summary>
    ///  Gets an awaiter for the installation request.
    /// </summary>

    public partial TaskAwaiter<object> GetAwaiter() ;

    /// <summary>
    /// Cancels the installation request.
    /// </summary>

    public  partial  void Cancel() ;

    /// <summary>
    ///  Asynchronously cancels the installation request.
    /// </summary>

    public partial  Task CancelAsync() ;

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>

    public partial  void Dispose() ;

    /// <summary>
    /// Cleanup resources held by the installation request.
    /// </summary>

    ~Request() => Dispose();
}