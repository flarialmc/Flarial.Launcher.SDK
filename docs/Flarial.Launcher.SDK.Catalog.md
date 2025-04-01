# `Flarial.Launcher.SDK.Catalog`

> [!TIP]
> Consider accquiring an instance of `Catalog` at startup & caching it globally.

Provides methods to manage Minecraft versions compatible with Flarial Client.

- [`Catalog.GetAsync()`](#cataloggetasync)

- [`Catalog.UriAsync(string)`](#cataloguriasyncstring)

- [`Catalog.CompatibleAsync()`](#catalogcompatibleasync)

- [`Catalog.GetEnumerator()`](#cataloggetenumerator)

- [`Catalog.FrameworksAsync()`](#catalogframeworksasync)

- [`Catalog.InstallAsync(string, Action<int>)`](#cataloginstallasyncstring-actionint)

## `Catalog.GetAsync()`

 Asynchronously gets a list of versions.
            
- Returns: A list of versions supported by Flarial Client.

## `Catalog.UriAsync(string)`

Asynchronously resolves a download link for the specified version.
    
- Parameter: The version to resolve.

- Returns: The download link for the specified version.

## `Catalog.CompatibleAsync()`

Asynchronously checks if the installed version of Minecraft Bedrock Edition is compatible with Flarial.

- Returns: A boolean value that represents compatibility.

## `Catalog.GetEnumerator()`

Enumerates versions present in the catalog.

## `Catalog.FrameworksAsync()`

Asynchronously installs frameworks required by Minecraft: Bedrock Edition.

## `Catalog.InstallAsync(string, Action<int>)`

> [!IMPORTANT]
> When binding a callback, consider invoking via a thread's dispatcher.<br>
> This will ensure it executes on the correct synchorization context.

Asynchronously starts the installation of a version.

- Parameters:

    - `string`: The version to be installed.

    - `Action<int>`: Callback for installation progress.

- Returns: An installation request.