# Extensions for MvvmCross
![GitHub](https://img.shields.io/github/license/SByteDev/Net.MvvmCross.Extensions.svg)
![Nuget](https://img.shields.io/nuget/v/SByteDev.MvvmCross.Extensions.svg)
[![Build Status](https://img.shields.io/bitrise/049bff8cefbc2c57/develop?label=development&token=nvM63a6qHcxTVxhZKMDb2g&branch)](https://app.bitrise.io/app/049bff8cefbc2c57)
[![Build Status](https://img.shields.io/bitrise/049bff8cefbc2c57/master?label=production&token=nvM63a6qHcxTVxhZKMDb2g&branch)](https://app.bitrise.io/app/049bff8cefbc2c57)
[![codecov](https://codecov.io/gh/SByteDev/Net.MvvmCross.Extensions/branch/master/graph/badge.svg)](https://codecov.io/gh/SByteDev/Net.MvvmCross.Extensions)
[![CodeFactor](https://www.codefactor.io/repository/github/sbytedev/net.mvvmcross.extensions/badge)](https://www.codefactor.io/repository/github/sbytedev/net.mvvmcross.extensions)

Extensions is a .Net Standard library with common extensions and helpers for [MvvmCross](https://github.com/MvvmCross/MvvmCross).

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

### IEnumerable Extensions
To transform a two-dimensional collection or an `ObservableCollection` into a single-dimensional collection and keep the `CollectionChanged` events:

```cs
Items = new ObservableCollection<ObservableCollection<int>>
{
    new ObservableCollection<int> { 1, 2 },
    new ObservableCollection<int> { 3 }
};

IEnumerable<int> flatItems = Items.ObservableFlatten();
```

To transform each element of a collection into a new form and keep the `CollectionChanged` events:

```cs
Items = new ObservableCollection<int> {1, 2, 3, 4, 5, 6};

IEnumerable<string> wrappedItems = Items.ObservableSelect(item => item.ToString());
```

### INotifyPropertyChanged Extensions
To subscribe for changes of `INotifyPropertyChanged` properties:

```cs
Subscription = this.WeakSubscribe(EventHandler, () => PropertyOne, () => PropertyTwo);
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update the tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
