# `Flarial.Launcher.SDK.Request`

Represents an installation request for a version.

- [`Request.GetAwaiter()`](#requestgetawaiter)

- [`Request.Cancel()`](#requestcancel)

- [`Request.CancelAsync()`](#requestcancelasync)

- [`Request.Dispose()`](#requestdispose)

## `Request.GetAwaiter()`
Gets an awaiter for the installation request.

## `Request.Cancel()`
Cancels the installation request.

> [!TIP]
> Consider using synchronous cancellation when an application is shutting down.

## `Request.CancelAsync()`
Asynchronously cancels the installation request.

> [!TIP]
> Consider using asynchronous cancellation when an application is running.