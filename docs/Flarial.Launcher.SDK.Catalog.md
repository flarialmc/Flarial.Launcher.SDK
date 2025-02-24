# `Flarial.Launcher.SDK.Catalog`

> [!TIP]
> Consider accquiring an instance of `Catalog` & caching it globally.

Provides methods to manage Minecraft versions compatible with Flarial Client.

- [`Catalog.GetAsync()`](#cataloggetasync)

- [`Catalog.CompatibleAsync()`](#catalogcompatibleasync)

- [`Catalog.GetEnumerator()`](#cataloggetenumerator)

- [`Catalog.InstallAsync(string, Action<int>)`](#cataloginstallasyncstring-actionint)

## `Catalog.GetAsync()`

 Asynchronously gets a list of versions.
            
- Returns: A list of versions supported by Flarial Client.

## `Catalog.CompatibleAsync()`

Checks if the installed version of Minecraft Bedrock Edition is compatible with Flarial.

- Returns: A boolean value that represents compatibility.

## `Catalog.GetEnumerator()`

Enumerates versions present in the catalog.

## `Catalog.InstallAsync(string, Action<int>)`

Asynchronously starts the installation of a version.

- Parameters:

    - `string`: The version to be installed.

    - `Action<int>`: Callback for installation progress.

- Returns: An installation request.

> [!IMPORTANT]
> When binding a callback, consider invoking via a thread's dispatcher.<br>
> This will ensure it executes on the correct synchorization context.