using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.WeakSubscription;
using SByte.Common.Extensions;

namespace SByte.MvvmCross.Extensions
{
    public static class CommandExtensions
    {
        public static async Task<bool> SafeExecuteAsync(this ICommand command, object parameter = null)
        {
            if (!command.SafeCanExecute(parameter))
            {
                return false;
            }

            if (command is IMvxAsyncCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(parameter).ConfigureAwait(false);
            }
            else
            {
                command.Execute(parameter);
            }

            return true;
        }

        public static IDisposable RelayOn(
            this ICommand command,
            INotifyPropertyChanged notifyPropertyChanged,
            params Expression<Func<object>>[] properties
        )
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (notifyPropertyChanged == null)
            {
                throw new ArgumentNullException(nameof(notifyPropertyChanged));
            }

            if (!(command is IMvxCommand mvxCommand))
            {
                return null;
            }

            return notifyPropertyChanged.WeakSubscribe((_, args) =>
            {
                if (properties.All(item =>
                    notifyPropertyChanged.GetPropertyNameFromExpression(item) != args.PropertyName))
                {
                    return;
                }

                mvxCommand.RaiseCanExecuteChanged();
            });
        }
    }
}