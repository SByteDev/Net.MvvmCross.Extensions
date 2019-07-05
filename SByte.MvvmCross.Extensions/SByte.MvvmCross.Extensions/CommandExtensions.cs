using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Commands;
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
    }
}