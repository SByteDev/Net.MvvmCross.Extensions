using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MvvmCross.ViewModels;
using NUnit.Framework;
using SByteDev.Common.Extensions;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    [TestFixture]
    public class FlatObservableCollectionTests
    {
        private static int GetFlatItemIndex<T>(IEnumerable<IEnumerable<T>> items, int section, int index)
        {
            return GetFlatSectionIndex(items, section) + index;
        }

        private static int GetFlatSectionIndex<T>(IEnumerable<IEnumerable<T>> sections, int section)
        {
            return sections.Take(section).Sum(item => item.Count());
        }

        private static IEnumerable<T> GetFlatItems<T>(IEnumerable<IEnumerable<T>> sections)
        {
            return sections.SelectMany(item => item).ToArray();
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
                    Assert.Throws<ArgumentNullException>(() => new FlatObservableCollection<object>(null));
                }
            }

            [Test]
            public void ShouldContainInitialItems()
            {
                var items = new[] {new[] {1, 2}, new[] {3}};

                var sut = new FlatObservableCollection<int>(items);

                Assert.AreEqual(3, sut.Count);
                Assert.IsTrue(sut.Contains(1));
                Assert.IsTrue(sut.Contains(2));
                Assert.IsTrue(sut.Contains(3));
            }

            [Test]
            public void InitialItemsShouldBeInProvidedOrder()
            {
                var items = new[] {new[] {1, 2}, new[] {3}};

                var sut = new FlatObservableCollection<int>(items);

                Assert.AreEqual(0, sut.IndexOf(1));
                Assert.AreEqual(1, sut.IndexOf(2));
                Assert.AreEqual(2, sut.IndexOf(3));
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
                    var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}});

                    var sut = new FlatObservableCollection<int>(items);

                    sut.CollectionChanged += (_, args) => events.Add(args);

                    sut.Dispose();

                    items.Add(new[] {1});
                    items.Remove(new[] {1});

                    Assert.AreEqual(0, events.Count);
                }
            }
        }

        [TestFixture]
        public class WhenSectionsAdded : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(new[] {4})]
            [TestCase(new[] {4}, new[] {4})]
            [TestCase(new[] {4}, new[] {5}, new[] {6})]
            public void AddEventShouldBeRaised(params int[][] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}});
                var flatIndex = GetFlatSectionIndex(items, items.Count);
                var flatItems = GetFlatItems(newItems);

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.AddRange(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
                Assert.AreEqual(flatIndex, events[0].NewStartingIndex);
                Assert.AreEqual(flatItems, events[0].NewItems);
            }

            [Test]
            [TestCase(new[] {4})]
            [TestCase(new[] {4}, new[] {4})]
            [TestCase(new[] {4}, new[] {5}, new[] {6})]
            public void CollectionShouldBeUpdated(params int[][] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}});
                var flatItems = GetFlatItems(newItems);
                var flatCount = GetFlatItems(items).Count() + flatItems.Count();

                var sut = new FlatObservableCollection<int>(items);

                items.AddRange(newItems);

                Assert.AreEqual(flatCount, sut.Count);
                Assert.AreEqual(flatItems, sut.TakeLast(newItems.Length));
            }
        }

        [TestFixture]
        public class WhenSectionInserted : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0, new[] {4})]
            [TestCase(1, new[] {4, 4})]
            [TestCase(2, new[] {4, 5, 6})]
            public void AddEventShouldBeRaised(int index, int[] newItem)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new ObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}});
                var flatIndex = GetFlatSectionIndex(items, index);
                var flatItems = GetFlatItems(new[] {newItem});

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.Insert(index, newItem);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
                Assert.AreEqual(flatIndex, events[0].NewStartingIndex);
                Assert.AreEqual(flatItems, events[0].NewItems);
            }

            [Test]
            [TestCase(0, new[] {4})]
            [TestCase(1, new[] {4, 4})]
            [TestCase(2, new[] {4, 5, 6})]
            public void CollectionShouldBeUpdated(int index, int[] newItem)
            {
                Setup();

                var items = new ObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}});
                var flatIndex = GetFlatSectionIndex(items, index);
                var flatItems = GetFlatItems(new[] {newItem});
                var flatCount = GetFlatItems(items).Count() + flatItems.Count();

                var sut = new FlatObservableCollection<int>(items);

                items.Insert(index, newItem);

                Assert.AreEqual(flatCount, sut.Count);
                Assert.AreEqual(flatItems, sut.Take(flatIndex, newItem.Length));
            }
        }

        [TestFixture]
        public class WhenSectionsRemoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0, 1)]
            [TestCase(1, 1)]
            [TestCase(2, 1)]
            [TestCase(0, 3)]
            public void RemoveEventShouldBeRaised(int index, int count)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});
                var flatIndex = GetFlatSectionIndex(items, index);
                var flatItems = GetFlatItems(items.Take(index, count));

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.RemoveRange(index, count);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, events[0].Action);
                Assert.AreEqual(flatIndex, events[0].OldStartingIndex);
                Assert.AreEqual(flatItems, events[0].OldItems);
            }

            [Test]
            [TestCase(0, 1)]
            [TestCase(1, 1)]
            [TestCase(2, 1)]
            [TestCase(0, 3)]
            public void CollectionShouldBeUpdated(int index, int count)
            {
                Setup();

                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});
                var flatItems = GetFlatItems(items.Take(index, count)).ToArray();
                var flatCount = GetFlatItems(items).Count() - flatItems.Length;

                var sut = new FlatObservableCollection<int>(items);

                items.RemoveRange(index, count);

                Assert.AreEqual(flatCount, sut.Count);
                CollectionAssert.IsNotSubsetOf(flatItems, sut);
            }
        }

        [TestFixture]
        public class WhenSectionsReplaced : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(new[] {4})]
            [TestCase(new[] {4}, new[] {4})]
            [TestCase(new[] {4}, new[] {5}, new[] {6})]
            public void ResetEventShouldBeRaised(params int[][] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.ReplaceWith(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, events[0].Action);
            }

            [Test]
            [TestCase(new[] {4})]
            [TestCase(new[] {4}, new[] {4})]
            [TestCase(new[] {4}, new[] {5}, new[] {6})]
            public void CollectionShouldBeUpdated(params int[][] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});
                var flatItems = GetFlatItems(newItems).ToArray();
                var flatCount = flatItems.Length;

                var sut = new FlatObservableCollection<int>(items);

                items.ReplaceWith(newItems);

                Assert.AreEqual(flatCount, sut.Count);
                Assert.AreEqual(flatItems, sut);
            }
        }

        [TestFixture]
        public class WhenSectionsMoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0, 1)]
            [TestCase(1, 1)]
            [TestCase(2, 1)]
            public void MoveEventShouldBeRaised(int oldIndex, int newIndex)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();
                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});
                var flatOldIndex = GetFlatSectionIndex(items, oldIndex);

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items.Move(oldIndex, newIndex);

                var flatNewIndex = GetFlatSectionIndex(items, newIndex);
                var flatNewItems = GetFlatItems(items.Take(newIndex, 1));

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Move, events[0].Action);
                Assert.AreEqual(flatOldIndex, events[0].OldStartingIndex);
                Assert.AreEqual(flatNewIndex, events[0].NewStartingIndex);
                Assert.AreEqual(flatNewItems, events[0].NewItems);
                Assert.AreEqual(flatNewItems, events[0].OldItems);
            }

            [Test]
            [TestCase(0, 1)]
            [TestCase(1, 1)]
            [TestCase(2, 1)]
            public void CollectionShouldBeUpdated(int oldIndex, int newIndex)
            {
                Setup();

                var items = new MvxObservableCollection<int[]>(new[] {new[] {1, 2}, new[] {3}, new[] {4, 5, 6}});
                var flatOldItems = GetFlatItems(items).ToArray();
                var flatOldCount = flatOldItems.Length;

                var sut = new FlatObservableCollection<int>(items);

                items.Move(oldIndex, newIndex);

                var flatNewItems = GetFlatItems(items).ToArray();
                var flatNewCount = flatNewItems.Length;

                Assert.AreEqual(flatOldCount, sut.Count);
                Assert.AreEqual(flatNewCount, sut.Count);
                Assert.AreEqual(flatNewItems, sut);
            }
        }

        [TestFixture]
        public class WhenItemsAdded : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0, new[] {4})]
            [TestCase(0, new[] {4, 4})]
            [TestCase(1, new[] {4, 5, 6})]
            public void AddEventShouldBeRaised(int section, params int[] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3
                    }
                };

                var flatIndex = GetFlatItemIndex(items, section, items[section].Count);

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items[section].AddRange(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
                Assert.AreEqual(flatIndex, events[0].NewStartingIndex);
                Assert.AreEqual(newItems, events[0].NewItems);
            }

            [Test]
            [TestCase(0, new[] {4})]
            [TestCase(0, new[] {4, 4})]
            [TestCase(1, new[] {4, 5, 6})]
            public void CollectionShouldBeUpdated(int section, params int[] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3
                    }
                };

                var flatIndex = GetFlatItemIndex(items, section, items[section].Count);
                var flatCount = GetFlatItems(items).Count() + newItems.Length;

                var sut = new FlatObservableCollection<int>(items);

                items[section].AddRange(newItems);

                Assert.AreEqual(flatCount, sut.Count);
                Assert.AreEqual(newItems, sut.Take(flatIndex, newItems.Length));
            }
        }

        [TestFixture]
        public class WhenItemsRemoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0, 1, 1)]
            [TestCase(1, 2, 2)]
            [TestCase(0, 0, 2)]
            [TestCase(1, 0, 4)]
            public void RemoveEventShouldBeRaised(int section, int start, int count)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var flatIndex = GetFlatItemIndex(items, section, start);
                var oldItems = items[section].Take(start, count).ToArray();

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items[section].RemoveRange(start, count);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, events[0].Action);
                Assert.AreEqual(flatIndex, events[0].OldStartingIndex);
                Assert.AreEqual(oldItems, events[0].OldItems);
            }

            [Test]
            [TestCase(0, 1, 1)]
            [TestCase(1, 2, 2)]
            [TestCase(0, 0, 2)]
            [TestCase(1, 0, 4)]
            public void CollectionShouldBeUpdated(int section, int start, int count)
            {
                Setup();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var flatCount = GetFlatItems(items).Count() - count;
                var oldItems = items[section].Take(start, count).ToArray();

                var sut = new FlatObservableCollection<int>(items);

                items[section].RemoveRange(start, count);

                Assert.AreEqual(flatCount, sut.Count);
                CollectionAssert.IsNotSubsetOf(oldItems, sut);
            }
        }

        [TestFixture]
        public class WhenItemsReplaced : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(0)]
            [TestCase(1, 7, 8)]
            public void ResetEventShouldBeRaised(int section, params int[] newItems)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items[section].ReplaceWith(newItems);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, events[0].Action);
            }

            [Test]
            [TestCase(0)]
            [TestCase(1, 7, 8)]
            public void CollectionShouldBeUpdated(int section, params int[] newItems)
            {
                Setup();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var sut = new FlatObservableCollection<int>(items);

                items[section].ReplaceWith(newItems);

                var flatItems = GetFlatItems(items).ToArray();
                var flatCount = flatItems.Length;

                Assert.AreEqual(flatCount, sut.Count);
                Assert.AreEqual(flatItems, sut);
            }
        }

        [TestFixture]
        public class WhenItemsMoved : MvxObservableCollectionTestBase
        {
            [Test]
            [TestCase(1, 1, 2)]
            [TestCase(1, 2, 2)]
            [TestCase(1, 2, 1)]
            public void MoveEventShouldBeRaised(int section, int oldIndex, int newIndex)
            {
                Setup();

                var events = new List<NotifyCollectionChangedEventArgs>();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var flatOldIndex = GetFlatItemIndex(items, section, oldIndex);
                var changedItems = items[section].Take(oldIndex, 1).ToArray();

                var sut = new FlatObservableCollection<int>(items);

                sut.CollectionChanged += (_, args) => events.Add(args);

                items[section].Move(oldIndex, newIndex);

                var flatNewIndex = GetFlatItemIndex(items, section, newIndex);

                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Move, events[0].Action);
                Assert.AreEqual(flatOldIndex, events[0].OldStartingIndex);
                Assert.AreEqual(flatNewIndex, events[0].NewStartingIndex);
                Assert.AreEqual(changedItems, events[0].NewItems);
                Assert.AreEqual(changedItems, events[0].OldItems);
            }

            [Test]
            [TestCase(1, 1, 2)]
            [TestCase(1, 2, 2)]
            [TestCase(1, 2, 1)]
            public void CollectionShouldBeUpdated(int section, int oldIndex, int newIndex)
            {
                Setup();

                var items = new MvxObservableCollection<MvxObservableCollection<int>>
                {
                    new MvxObservableCollection<int>
                    {
                        1, 2
                    },
                    new MvxObservableCollection<int>
                    {
                        3, 4, 5, 6
                    }
                };

                var flatOldItems = GetFlatItems(items).ToArray();
                var flatOldCount = flatOldItems.Length;

                var sut = new FlatObservableCollection<int>(items);

                items[section].Move(oldIndex, newIndex);

                var flatNewItems = GetFlatItems(items).ToArray();
                var flatNewCount = flatNewItems.Length;

                Assert.AreEqual(flatOldCount, sut.Count);
                Assert.AreEqual(flatNewCount, sut.Count);
                Assert.AreEqual(flatNewItems, sut);
            }
        }
    }
}