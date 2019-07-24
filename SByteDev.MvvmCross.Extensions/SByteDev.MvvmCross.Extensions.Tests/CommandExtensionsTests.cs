using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Tests;
using NSubstitute;
using NUnit.Framework;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    [TestFixture]
    public class CommandExtensionsTests
    {
        [TestFixture]
        public class WhenSafeExecuteAsyncCalled
        {
            [TestFixture]
            public class AndTheCommandIsNull
            {
                [Test]
                public void ExceptionsShouldNotBeThrown()
                {
                    const int parameter = 1;
                    Assert.DoesNotThrowAsync(() => default(ICommand).SafeExecuteAsync(parameter));
                }

                [Test]
                public async Task ExecuteShouldNotBeCalled()
                {
                    const int parameter = 1;
                    var command = Substitute.For<ICommand>();

                    await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                    command.DidNotReceive().Execute(Arg.Any<object>());
                }
            }

            [TestFixture]
            public class AndTheCommandIsNotExecutable
            {
                [Test]
                public async Task ShouldReturnFalse()
                {
                    const int parameter = 1;
                    var command = Substitute.For<ICommand>();

                    var result = await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                    Assert.IsFalse(result);
                }

                [Test]
                public async Task ExecuteShouldNotBeCalled()
                {
                    const int parameter = 1;
                    var command = Substitute.For<ICommand>();

                    await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                    command.DidNotReceive().Execute(Arg.Any<object>());
                }
            }

            [TestFixture]
            public class AndTheCommandIsExecutable
            {
                [TestFixture]
                public class AndTheCommandIsAsyncCommand
                {
                    [Test]
                    public async Task ExecuteAsyncShouldBeCalled()
                    {
                        const int parameter = 1;
                        var command = Substitute.For<IMvxAsyncCommand>();
                        command.CanExecute(Arg.Any<object>()).Returns(true);

                        await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                        await command.Received().ExecuteAsync(Arg.Is(parameter)).ConfigureAwait(false);
                    }

                    [Test]
                    public async Task ShouldReturnTrue()
                    {
                        const int parameter = 1;
                        var command = Substitute.For<IMvxAsyncCommand>();
                        command.CanExecute(Arg.Any<object>()).Returns(true);

                        var result = await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                        Assert.IsTrue(result);
                    }
                }

                [TestFixture]
                public class AndTheCommandIsNotAsyncCommand
                {
                    [Test]
                    public async Task ExecuteShouldBeCalled()
                    {
                        const int parameter = 1;
                        var command = Substitute.For<ICommand>();
                        command.CanExecute(Arg.Any<object>()).Returns(true);

                        await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                        command.Received().Execute(Arg.Is(parameter));
                    }

                    [Test]
                    public async Task ShouldReturnTrue()
                    {
                        const int parameter = 1;
                        var command = Substitute.For<ICommand>();
                        command.CanExecute(Arg.Any<object>()).Returns(true);

                        var result = await command.SafeExecuteAsync(parameter).ConfigureAwait(false);

                        Assert.IsTrue(result);
                    }
                }
            }
        }

        [TestFixture]
        public class WhenCancelExecutionCalled
        {
            [TestFixture]
            public class AndTheCommandIsNull
            {
                [Test]
                public void ExceptionsShouldNotBeThrown()
                {
                    Assert.DoesNotThrow(() => default(ICommand).CancelExecution());
                }

                [Test]
                public void ShouldReturnFalse()
                {
                    Assert.IsFalse(default(ICommand).CancelExecution());
                }
            }

            [TestFixture]
            public class AndTheCommandIsNotMvxAsyncCommand
            {
                [Test]
                public void ShouldReturnFalse()
                {
                    Assert.IsFalse(Substitute.For<ICommand>().CancelExecution());
                }
            }

            [TestFixture]
            public class AndTheCommandIsNotRunning
            {
                [Test]
                public void ShouldReturnFalse()
                {
                    var command = new MvxAsyncCommand(() => Task.Delay(TimeSpan.FromMilliseconds(5)));

                    Assert.IsFalse(command.CancelExecution());
                }
            }

            [TestFixture]
            public class AndTheCommandIsRunning : MvxIoCSupportingTest
            {
                [Test]
                public void ShouldReturnTrue()
                {
                    var command = new MvxAsyncCommand(() => Task.Delay(TimeSpan.FromSeconds(1)));

                    command.Execute();

                    Assert.IsTrue(command.CancelExecution());
                }

                [Test]
                public void ShouldCancelCommandExecution()
                {
                    Setup();

                    var task = default(Task);
                    var command = new MvxAsyncCommand(
                        cancellationToken => task = Task.Delay(TimeSpan.FromSeconds(1), cancellationToken)
                    );

                    command.Execute();
                    command.CancelExecution();

                    Assert.IsTrue(task.IsCanceled);
                }
            }
        }

        [TestFixture]
        public class WhenRelayOnCalled
        {
            [TestFixture]
            public class AndTheCommandIsNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    Assert.Throws<ArgumentNullException>(() => default(ICommand).RelayOn(null));
                }
            }

            [TestFixture]
            public class AndTheNotifyPropertyChangedIsNull
            {
                [Test]
                public void ExceptionShouldBeThrown()
                {
                    var command = Substitute.For<ICommand>();

                    Assert.Throws<ArgumentNullException>(() => command.RelayOn(null));
                }
            }

            [TestFixture]
            public class AndTheCommandIsNotMvxCommand
            {
                [Test]
                public void ShouldReturnNull()
                {
                    var notifyPropertyChanged = Substitute.For<INotifyPropertyChanged>();
                    var command = Substitute.For<ICommand>();

                    Assert.IsNull(command.RelayOn(notifyPropertyChanged));
                }
            }

            [TestFixture]
            public class AndTheCommandIsMvxCommand
            {
                // ReSharper disable once MemberCanBePrivate.Global
                public interface ITestNotifyPropertyChanged : INotifyPropertyChanged
                {
                    string FirstProperty { get; }
                    string SecondProperty { get; }
                }

                [Test]
                public void ShouldNotReturnNull()
                {
                    var notifyPropertyChanged = Substitute.For<INotifyPropertyChanged>();
                    var command = Substitute.For<IMvxCommand>();

                    Assert.IsNotNull(command.RelayOn(notifyPropertyChanged));
                }

                [TestFixture]
                public class AndSuitablePropertyChangedFired
                {
                    [Test]
                    public void ShouldRaiseCanExecuteChanged()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var command = Substitute.For<IMvxCommand>();

                        var _ = command.RelayOn(notifyPropertyChanged, () => notifyPropertyChanged.FirstProperty);

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(
                                nameof(ITestNotifyPropertyChanged.FirstProperty)
                            ));

                        command.Received().RaiseCanExecuteChanged();
                    }

                    [TestFixture]
                    public class AndSubscriptionIsDisposed
                    {
                        [Test]
                        public void ShouldNotRaiseCanExecuteChanged()
                        {
                            var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                            var command = Substitute.For<IMvxCommand>();

                            var subscription = command.RelayOn(
                                notifyPropertyChanged,
                                () => notifyPropertyChanged.FirstProperty
                            );

                            subscription.Dispose();

                            notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                                new PropertyChangedEventArgs(
                                    nameof(ITestNotifyPropertyChanged.FirstProperty)
                                ));

                            command.DidNotReceive().RaiseCanExecuteChanged();
                        }
                    }
                }

                [TestFixture]
                public class AndNotSuitablePropertyChangedFired
                {
                    [Test]
                    public void ShouldRaiseCanExecuteChanged()
                    {
                        var notifyPropertyChanged = Substitute.For<ITestNotifyPropertyChanged>();
                        var command = Substitute.For<IMvxCommand>();

                        var _ = command.RelayOn(notifyPropertyChanged, () => notifyPropertyChanged.FirstProperty);

                        notifyPropertyChanged.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
                            new PropertyChangedEventArgs(
                                nameof(ITestNotifyPropertyChanged.SecondProperty)
                            ));

                        command.DidNotReceive().RaiseCanExecuteChanged();
                    }
                }
            }
        }
    }
}