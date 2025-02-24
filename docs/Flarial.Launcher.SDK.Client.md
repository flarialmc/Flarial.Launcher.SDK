# `Flarial.Launcher.SDK.Client`

Provides methods to interact with Flarial Client's dynamic link library.

- [`Client.DownloadAsync(bool, Action<nint>)`](#clientdownloadasyncbool-actionnint)

- [`Client.LaunchAsync(bool)`](#clientlaunchasyncbool)

## `Client.DownloadAsync(bool, Action<nint>)`

Asynchronously download Flarial Client's dynamic link library.

- Parameters:

    - `bool`: Specify `true` to download Flarial Client's Beta.

    - `Action<int>`: Callback for download progress.

> [!NOTE]
> When binding a callback, consider invoking via a thread's dispatcher.<br>
> This will ensure it executes on the correct synchorization context.

## `Client.LaunchAsync(bool)`

Asynchronously launch Minecraft & load Flarial Client's dynamic link library.

- Parameters:

    - `bool`: Specify `true` to use Flarial Client's Beta.