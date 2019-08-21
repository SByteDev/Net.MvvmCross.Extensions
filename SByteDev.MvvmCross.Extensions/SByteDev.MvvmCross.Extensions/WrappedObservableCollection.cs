using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MvvmCross.Base;
using MvvmCross.WeakSubscription;

namespace SByteDev.MvvmCross.Extensions
{
    public class WrappedObservableCollection<TItem, TWrappedItem>
        : IReadOnlyList<TWrappedItem>, INotifyCollectionChanged, IDisposable
    {
        private readonly Func<TItem, TWrappedItem> _factory;
        private readonly IEnumerable<TItem> _sourceItems;

        private List<TWrappedItem> _items;
        private IDisposable _subscription;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _items.Count;

        public TWrappedItem this[int index] => _items[index];

        public WrappedObservableCollection(IEnumerable<TItem> source, Func<TItem, TWrappedItem> factory)
        {
            _sourceItems = source ?? throw new ArgumentNullException(nameof(source));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            Initialize();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Initialize()
        {
            _items = new List<TWrappedItem>();

            if (_sourceItems is INotifyCollectionChanged notifyCollectionChanged)
            {
                _subscription = notifyCollectionChanged.WeakSubscribe(OnSourceChanged);
            }

            Reset();
        }

        private void Clear()
        {
            var items = _items;

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                item.DisposeIfDisposable();
            }

            items.Clear();
        }

        private TWrappedItem GetWrappedItem(TItem item)
        {
            return _factory(item);
        }

        private TWrappedItem GetWrappedItem(int index)
        {
            return GetWrappedItem(_sourceItems.ElementAt(index));
        }

        protected virtual NotifyCollectionChangedEventArgs Add(NotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            var items = new TWrappedItem[args.NewItems.Count];

            for (var i = 0; i < items.Length; i++)
            {
                var item = GetWrappedItem(i + index);

                _items.Insert(i + index, item);

                items[i] = item;
            }

            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index);
        }

        protected virtual NotifyCollectionChangedEventArgs Remove(NotifyCollectionChangedEventArgs args)
        {
            var index = args.OldStartingIndex;
            var items = new TWrappedItem[args.OldItems.Count];

            for (var i = 0; i < items.Length; i++)
            {
                var item = _items[index];

                _items.RemoveAt(index);

                items[i] = item;

                item.DisposeIfDisposable();
            }

            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);
        }

        protected virtual NotifyCollectionChangedEventArgs Move(NotifyCollectionChangedEventArgs args)
        {
            var fromIndex = args.OldStartingIndex;
            var toIndex = args.NewStartingIndex;
            var items = new TWrappedItem[args.OldItems.Count];

            for (var i = 0; i < items.Length; i++)
            {
                var item = _items[i];

                _items.RemoveAt(fromIndex);

                items[i] = item;

                item.DisposeIfDisposable();
            }

            for (var i = 0; i < items.Length; i++)
            {
                var item = GetWrappedItem(i + toIndex);

                _items.Insert(i + toIndex, item);

                items[i] = item;
            }

            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, items, toIndex, fromIndex);
        }

        protected virtual NotifyCollectionChangedEventArgs Replace(NotifyCollectionChangedEventArgs args)
        {
            var fromIndex = args.OldStartingIndex;
            var toIndex = args.NewStartingIndex;
            var items = new TWrappedItem[args.OldItems.Count];

            for (var i = 0; i < items.Length; i++)
            {
                var item = _items[i];

                _items.RemoveAt(fromIndex);

                items[i] = item;

                item.DisposeIfDisposable();
            }

            for (var i = 0; i < items.Length; i++)
            {
                var item = GetWrappedItem(i + toIndex);

                _items.Insert(i + toIndex, item);

                items[i] = item;
            }

            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, items, toIndex,
                fromIndex);
        }

        protected virtual NotifyCollectionChangedEventArgs Reset()
        {
            Clear();

            _items.AddRange(_sourceItems.Select(GetWrappedItem));

            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        }

        private void OnSourceChanged(object _, NotifyCollectionChangedEventArgs args)
        {
            var newArgs = Update(args);

            if (newArgs == null)
            {
                return;
            }

            RaiseCollectionChanged(newArgs);
        }

        private NotifyCollectionChangedEventArgs Update(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return Add(args);
                case NotifyCollectionChangedAction.Remove:
                    return Remove(args);
                case NotifyCollectionChangedAction.Move:
                    return Move(args);
                case NotifyCollectionChangedAction.Replace:
                    return Replace(args);
                case NotifyCollectionChangedAction.Reset:
                    return Reset();
                default:
                    throw new ArgumentOutOfRangeException(nameof(NotifyCollectionChangedEventArgs));
            }
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public IEnumerator<TWrappedItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Dispose()
        {
            Clear();

            _subscription?.Dispose();
            _subscription = null;
        }
    }
}