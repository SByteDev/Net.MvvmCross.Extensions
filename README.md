# Extensions for MvvmCross

## ICommand Extensions
1. `SafeExecuteAsync` safely invokes the command if this command can be executed in its current state. If this command is `IMvxAsyncCommand` it will be executed asynchronously.
2. `RelayOn` calls `IMvxCommand.RaiseCanExecuteChanged` when corresponding property or properties changed.