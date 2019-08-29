using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MvvmCross.ViewModels;
using NUnit.Framework;
using SByteDev.Common.Extensions;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    [TestFixture]
    public class WrappedObservableCollectionTests
    {
        private static string Factory(int item)
        {
            return item.ToString();
        }

        private static IEnumerable<string> Transform(IEnumerable<int> items)
        {
            return items.Select(Factory);
        }

        [TestFixture]
        public class WhenCollectionCreated
        {
            [TestFixture]
            public class AndTheItemsSourceIsNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    Assert.Throws<ArgumentNullException>(() =>
                        new WrappedObservableCollection<object, object>(null, null)
                    );
                }
            }

            [TestFixture]
            public class AndTheFactoryIsNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    Assert.Throws<ArgumentNullException>(() =>
                        new WrappedObservableCollection<object, object>(Enumerable.Empty<object>(), null)
                    );
                }
            }

            [Test]
            public void ShouldContainInitialItems()
            {
                var items = new[] {1, 2, 3};

                var sut = new WrappedObservableCollection<int, string>(items, item => item.ToString());

                Assert.AreEqual(3, sut.Count);
                Assert.IsTrue(sut.Contains(1.ToString()));
                Assert.IsTrue(sut.Contains(2.ToString()));
                Assert.IsTrue(sut.Contains(3.ToString()));
            }

            [Test]
            public void InitialItemsShouldBeInProvidedOrder()
            {
                var items = new[] {1, 2, 3};

                var sut = new WrappedObservableCollection<int, string>(items, item => item.ToString());

                Assert.AreEqual(0, sut.IndexOf(1.ToString()));
                Assert.AreEqual(1, sut.IndexOf(2.ToString()));
                Assert.AreEqual(2, sut.IndexOf(3.ToString()));
            }
        }

        [TestFixture]
        public class WhenCollectionDisposed
        {
            [TestFixture]
            public class AndSourceChanged : MvxObservableCollectionTestBase
            {
                [Test]
                public void EventShouldNotBeRaised()
                {
                    Setup();

                    var events = new List<NotifyCollectionChangedEventArgs>();
                    var items = new MvxObservableCollection<int> {1, 2, 3};

                    var sut = new WrappedObservableCollection<int, string>(items, item => item.ToString());

                    sut.CollectionChanged += (_, args) => events.Add(args);

                    sut.Dispose();

                    items.Add(4);
                    items.Remove(4);

                    Assert.AreEqual(0, events.Count);
                }
            }
        }

        [TestFixture]
        public class WhenItemsAdded : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(4)]
            [TestCase(4, 4)]
            [TestCase(4, 5, 6)]
            public void AddEventShouldBeRaised(params int[] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int> {1, 2, 3};
                var index = items.Count;

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.AddRange(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
                Assert.AreEqual(index, events[0].NewStartingIndex);
                Assert.AreEqual(Transform(newItems), events[0].NewItems);
            }

            [Test]
            [TestCase(4)]
            [TestCase(4, 4)]
            [TestCase(4, 5, 6)]
            public void CollectionShouldBeUpdated(params int[] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<int> {1, 2, 3};
                var index = items.Count;

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                items.AddRange(newItems);

                Assert.AreEqual(items.Count, sut.Count);
                Assert.AreEqual(Transform(newItems), sut.Take(index, newItems.Length));
            }
        }

        [TestFixture]
        public class WhenItemsRemoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(1, 1)]
            [TestCase(2, 2)]
            [TestCase(0, 2)]
            [TestCase(0, 4)]
            public void RemoveEventShouldBeRaised(int start, int count)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};
                var oldItems = Transform(items.ToArray().Take(start, count));

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.RemoveRange(start, count);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, events[0].Action);
                Assert.AreEqual(start, events[0].OldStartingIndex);
                Assert.AreEqual(oldItems, events[0].OldItems);
            }

            [Test]
            [TestCase(1, 1)]
            [TestCase(2, 2)]
            [TestCase(0, 2)]
            [TestCase(0, 4)]
            public void CollectionShouldBeUpdated(int start, int count)
            {
                Setup();

                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};
                var oldItems = Transform(items.ToArray().Take(start, count));

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                items.RemoveRange(start, count);

                Assert.AreEqual(items.Count, sut.Count);
                CollectionAssert.IsNotSubsetOf(oldItems, sut);
            }
        }

        [TestFixture]
        public class WhenItemsReplaced : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(new int[0])]
            [TestCase(7, 8)]
            public void ResetEventShouldBeRaised(params int[] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.ReplaceWith(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, events[0].Action);
            }

            [Test]
            [TestCase(new int[0])]
            [TestCase(7, 8)]
            public void CollectionShouldBeUpdated(params int[] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                items.ReplaceWith(newItems);

                Assert.AreEqual(Transform(newItems), sut);
            }
        }

        [TestFixture]
        public class WhenItemsMoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(1, 2)]
            [TestCase(2, 2)]
            [TestCase(2, 1)]
            public void MoveEventShouldBeRaised(int oldIndex, int newIndex)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};
                var changedItems = new[] {Transform(items).ElementAt(oldIndex)};

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.Move(oldIndex, newIndex);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Move, events[0].Action);
                Assert.AreEqual(oldIndex, events[0].OldStartingIndex);
                Assert.AreEqual(newIndex, events[0].NewStartingIndex);
                Assert.AreEqual(changedItems, events[0].NewItems);
                Assert.AreEqual(changedItems, events[0].OldItems);
            }

            [Test]
            [TestCase(1, 2)]
            [TestCase(2, 2)]
            [TestCase(2, 1)]
            public void CollectionShouldBeUpdated(int oldIndex, int newIndex)
            {
                Setup();

                var items = new MvxObservableCollection<int> {1, 2, 3, 4, 5, 6};

                var sut = new WrappedObservableCollection<int, string>(items, Factory);

                items.Move(oldIndex, newIndex);

                Assert.AreEqual(Transform(items), sut);
            }
        }
    }
}