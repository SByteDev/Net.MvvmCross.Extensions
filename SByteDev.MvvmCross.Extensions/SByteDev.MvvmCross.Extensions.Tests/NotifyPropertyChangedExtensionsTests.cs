using System;
using System.ComponentModel;
using NSubstitute;
using NUnit.Framework;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    [TestFixture]
    public class NotifyPropertyChangedExtensionsTests
    {
        [TestFixture]
        public class WhenWeakSubscribeCalled
        {
            [TestFixture]
            public class AndTheNotifyPropertyChangedIsNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    Assert.Throws<ArgumentNullException>(() => default(INotifyPropertyChanged).WeakSubscribe(null));
                }
            }

            [TestFixture]
            public class AndThePropertiesAreNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        default(INotifyPropertyChanged).WeakSubscribe((_, __) => { }, null)
                    );
                }
            }

            [Test]
            public void ShouldNotReturnNull()
            {
                Assert.IsNotNull(Substitute.For<INotifyPropertyChanged>().WeakSubscribe((_, __) => { }));
            }

            [TestFixture]
            public class AndSuitablePropertyChangedFired
            {
                [TestFixture]
                public class AndSubscriptionIsAlive
                {
                    [Test]
                    public void ShouldRaiseCanExecuteChanged()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var isCalled = default(bool);

                        var _ = notifyPropertyChanged.WeakSubscribe(
                            (__, ___) => isCalled = true,
                            () => notifyPropertyChanged.FirstProperty
                        );

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(nameof(ITestNotifyPropertyChanged.FirstProperty))
                        );

                        Assert.IsTrue(isCalled);
                    }
                }

                [TestFixture]
                public class AndSubscriptionIsDisposed
                {
                    [Test]
                    public void ShouldNotCallEventHandler()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var isCalled = default(bool);

                        var subscription = notifyPropertyChanged.WeakSubscribe(
                            (_, __) => isCalled = true,
                            () => notifyPropertyChanged.FirstProperty
                        );

                        subscription.Dispose();

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(nameof(ITestNotifyPropertyChanged.FirstProperty))
                        );

                        Assert.IsFalse(isCalled);
                    }
                }
            }

            [TestFixture]
            public class AndNotSuitablePropertyChangedFired
            {
                [TestFixture]
                public class AndSubscriptionIsAlive
                {
                    [Test]
                    public void ShouldNotCallEventHandler()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var isCalled = default(bool);

                        var _ = notifyPropertyChanged.WeakSubscribe(
                            (__, ___) => isCalled = true,
                            () => notifyPropertyChanged.FirstProperty
                        );

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(nameof(ITestNotifyPropertyChanged.SecondProperty))
                        );

                        Assert.IsFalse(isCalled);
                    }
                }

                [TestFixture]
                public class AndSubscriptionIsDisposed
                {
                    [Test]
                    public void ShouldNotCallEventHandler()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var isCalled = default(bool);

                        var subscription = notifyPropertyChanged.WeakSubscribe(
                            (_, __) => isCalled = true,
                            () => notifyPropertyChanged.FirstProperty
                        );

                        subscription.Dispose();

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(nameof(ITestNotifyPropertyChanged.SecondProperty))
                        );

                        Assert.IsFalse(isCalled);
                    }
                }
            }

            [TestFixture]
            public class AndAllPropertiesChangedFired
            {
                [TestFixture]
                public class AndSubscriptionIsAlive
                {
                    [TestFixture]
                    public class AndPropertiesAreEmpty
                    {
                        [Test]
                        public void ShouldCallEventHandler()
                        {
                            var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                            var isCalled = default(bool);

                            var _ = notifyPropertyChanged.WeakSubscribe((__, ___) => isCalled = true);

                            notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                                new PropertyChangedEventArgs(string.Empty)
                            );

                            Assert.IsTrue(isCalled);
                        }
                    }
                }

                [TestFixture]
                public class AndSubscriptionIsDisposed
                {
                    [Test]
                    public void ShouldNotCallEventHandler()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var isCalled = default(bool);

                        var subscription = notifyPropertyChanged.WeakSubscribe(
                            (_, __) => isCalled = true,
                            () => notifyPropertyChanged.FirstProperty
                        );

                        subscription.Dispose();

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(string.Empty)
                        );

                        Assert.IsFalse(isCalled);
                    }
                }
            }
        }
    }
}