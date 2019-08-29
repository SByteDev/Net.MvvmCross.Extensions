using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using MvvmCross.Base;
using MvvmCross.WeakSubscription;
using SByteDev.Common.Extensions;

namespace SByteDev.MvvmCross.Extensions
{
    internal sealed class MvxNamedNotifyPropertiesChangedEventSubscription : MvxNotifyPropertyChangedEventSubscription
    {
        private readonly string[] _propertyNames;

        public MvxNamedNotifyPropertiesChangedEventSubscription(
            INotifyPropertyChanged source,
            EventHandler<PropertyChangedEventArgs> targetEventHandler,
            params Expression<Func<object>>[] properties
        )
            : base(source, targetEventHandler)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            _propertyNames = properties.Select(source.GetPropertyNameFromExpression).ToArray();
        }

        protected override Delegate CreateEventHandler()
        {
            return new PropertyChangedEventHandler(OnPropertyChanged);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_propertyNames.IsNullOrEmpty() || string.IsNullOrWhiteSpace(args.PropertyName))
            {
                OnSourceEvent(sender, args);

                return;
            }

            if (_propertyNames.All(item => item != args.PropertyName))
            {
                return;
            }

            OnSourceEvent(sender, args);
        }
    }
}