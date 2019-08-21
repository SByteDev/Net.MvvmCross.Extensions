using System;
using System.Threading.Tasks;
using MvvmCross.Base;
using MvvmCross.Tests;
using NSubstitute;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    public class MvxObservableCollectionTestBase : MvxIoCSupportingTest
    {
        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();

            var dispatcher = Substitute.For<IMvxMainThreadAsyncDispatcher>();
            dispatcher.ExecuteOnMainThreadAsync(Arg.Any<Action>(), Arg.Any<bool>())
                .Returns(args =>
                {
                    ((Action) args[0])();
                    return Task.CompletedTask;
                });
            dispatcher.ExecuteOnMainThreadAsync(Arg.Any<Func<Task>>(), Arg.Any<bool>())
                .Returns(args => ((Func<Task>) args[0])());
            dispatcher.IsOnMainThread.Returns(true);

            Ioc.RegisterSingleton(dispatcher);
        }
    }
}