using System;
using System.Collections.Generic;

namespace SByteDev.MvvmCross.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Transforms two-dimensional observable collection into the flatten one
        /// and provides notifications when items get added, removed, or when the whole list is refreshed.
        /// </summary>
        /// <param name="enumerable">A collection of values to transform.</param>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <returns>A flat observable collection
        /// whose elements are the result of two-dimensional -> single-dimensional conversion.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static IEnumerable<T> ObservableFlatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            return new FlatObservableCollection<T>(enumerable);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="enumerable">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element.</param>
        /// <typeparam name="TItem">The type of the elements of source.</typeparam>
        /// <typeparam name="TWrappedItem">The type of the value returned by selector.</typeparam>
        /// <returns>An observable collection whose elements are the result of invoking
        /// the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static IEnumerable<TWrappedItem> ObservableSelect<TItem, TWrappedItem>(
            this IEnumerable<TItem> enumerable,
            Func<TItem, TWrappedItem> selector
        )
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new WrappedObservableCollection<TItem, TWrappedItem>(enumerable, selector);
        }
    }
}