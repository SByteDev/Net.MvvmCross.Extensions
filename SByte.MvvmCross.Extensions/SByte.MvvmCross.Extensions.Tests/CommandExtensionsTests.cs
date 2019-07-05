using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Commands;
using NSubstitute;
using NUnit.Framework;

namespace SByte.MvvmCross.Extensions.Tests
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

                    Assert.False(result);
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

                        Assert.True(result);
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

                        Assert.True(result);
                    }
                }
            }
        }
    }
}