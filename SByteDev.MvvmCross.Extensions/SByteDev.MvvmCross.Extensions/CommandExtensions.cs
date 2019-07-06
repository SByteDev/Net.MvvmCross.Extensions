using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.WeakSubscription;
using SByteDev.Common.Extensions;

namespace SByteDev.MvvmCross.Extensions
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Safely invokes the command if this command can be executed in its current state.
        /// If this command is <c>IMvxAsyncCommand</c> it will be executed asynchronously.
        /// </summary>
        /// <param name="command">The command itself.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <returns>True if the command was invoked and False otherwise.</returns>
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

        /// <summary>
        /// Calls <c>RaiseCanExecuteChanged</c> on <c>IMvxCommand</c>
        /// when property from <paramref name="properties"/> changed.
        /// </summary>
        /// <param name="command">The target command.</param>
        /// <param name="notifyPropertyChanged">A container for properties.</param>
        /// <param name="properties">A list of properties to relay on.</param>
        /// <returns><c>IDisposable</c> that represents subscription.
        /// Can be disposed to stop listening for <c>PropertyChanged</c> event.
        /// Returns <c>null</c> if <paramref name="command"/> is not <c>IMvxCommand</c></returns>
        /// <exception cref="ArgumentNullException">If command or notifyPropertyChanged are <c>null</c>.</exception>
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