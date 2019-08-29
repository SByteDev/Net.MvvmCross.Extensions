using System;
using System.ComponentModel;
using System.Linq.Expressions;
using MvvmCross.Commands;

namespace SByteDev.MvvmCross.Extensions
{
    internal class RelayCommandSubscription : IDisposable
    {
        private readonly WeakReference<IMvxCommand> _commandReference;

        private IDisposable _subscription;

        public RelayCommandSubscription(
            IMvxCommand command,
            INotifyPropertyChanged propertyChanged,
            Expression<Func<object>>[] properties
        )
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (propertyChanged == null)
            {
                throw new ArgumentNullException(nameof(propertyChanged));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            _commandReference = new WeakReference<IMvxCommand>(command);

            _subscription = propertyChanged.WeakSubscribe(OnPropertyChanged, properties);
        }

        private void OnPropertyChanged(object _, PropertyChangedEventArgs args)
        {
            if (_commandReference.TryGetTarget(out var command))
            {
                command.RaiseCanExecuteChanged();
            }
            else
            {
                _subscription?.Dispose();
                _subscription = null;
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}