using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.WeakSubscription;

namespace SByteDev.MvvmCross.Extensions
{
    internal class RelayCommandSubscription : IDisposable
    {
        private IMvxCommand _command;
        private INotifyPropertyChanged _propertyChanged;
        private Expression<Func<object>>[] _properties;
        private IDisposable _subscription;

        public RelayCommandSubscription(
            IMvxCommand command,
            INotifyPropertyChanged propertyChanged,
            Expression<Func<object>>[] properties
        )
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _propertyChanged = propertyChanged ?? throw new ArgumentNullException(nameof(propertyChanged));
            _properties = properties ?? throw new ArgumentNullException(nameof(_properties));

            _subscription = _propertyChanged.WeakSubscribe(OnPropertyChanged);
        }

        private void OnPropertyChanged(object _, PropertyChangedEventArgs args)
        {
            var properties = _properties;
            var propertyChanged = _propertyChanged;

            if (properties == null || propertyChanged == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(args.PropertyName))
            {
                _command?.RaiseCanExecuteChanged();

                return;
            }

            if (properties.All(item => propertyChanged.GetPropertyNameFromExpression(item) != args.PropertyName))
            {
                return;
            }

            _command?.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            _command = null;
            _propertyChanged = null;
            _properties = null;

            _subscription?.Dispose();
            _subscription = null;
        }
    }
}