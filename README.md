# Extensions for MvvmCross
![GitHub](https://img.shields.io/github/license/SByteDev/Net.MvvmCross.Extensions.svg)
![Nuget](https://img.shields.io/nuget/v/SByteDev.MvvmCross.Extensions.svg)

Extensions is a .Net Standard library with common extensions and helpers or [MvvmCross](https://github.com/MvvmCross/MvvmCross).

## Installation

Use [NuGet](https://www.nuget.org) package manager to install this library.

```bash
Install-Package SByteDev.MvvmCross.Extensions
```

## Usage
```cs
using SByteDev.MvvmCross.Extensions;
```

### ICommand Extensions
To check the execution ability and run the `IMvxAsyncCommand` asynchronously:

```cs
await Command.SafeExecuteAsync(parameter);
```

To cancel the `IMvxAsyncCommand` execution with `IsRunning` check:

```cs
Command.CancelExecution();
```

To raise `CanExecuteChanged` when one of the target properties changed:

```cs
Subscription = Command.RelayOn(
    NotifyPropertyChanged,
    () => NotifyPropertyChanged.Property
);
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update the tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)