using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SByteDev.MvvmCross.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Subscribes to a set of properties of a given source.
        /// </summary>
        /// <param name="source">The source to subscribe on.</param>
        /// <param name="eventHandler">The callback.</param>
        /// <param name="properties">The set of properties to listen on.</param>
        /// <returns>Disposable that can be disposed to cancel a subscription.</returns>
        /// <exception cref="ArgumentNullException">If command or notifyPropertyChanged or properties are <c>null</c>.</exception>
        public static IDisposable WeakSubscribe(
            this INotifyPropertyChanged source,
            EventHandler<PropertyChangedEventArgs> eventHandler,
            params Expression<Func<object>>[] properties
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            return new MvxNamedNotifyPropertiesChangedEventSubscription(source, eventHandler, properties);
        }
    }
}