# `Flarial.Launcher.SDK.Launcher`

Provides methods to manage Flarial Client's launcher.

- [`Launcher.AvailableAsync()`](#launcheravailableasync)

- [`Launcher.UpdateAsync(Action<int>)`](#launcherupdateasyncactionint)

## `Launcher.AvailableAsync()`

> [!CAUTION]
> This method is deprecated.

Asynchronously check if a launcher update is available. 

## `Launcher.UpdateAsync(Action<int>)`

> [!IMPORTANT]
> When binding a callback, consider invoking via a thread's dispatcher.<br>
> This will ensure it executes on the correct synchorization context.

 Asynchronously check & if required, update the launcher.
     
- Parameter: Callback for update progress.
    
- Returns: A boolean value that represents the availability of an update.