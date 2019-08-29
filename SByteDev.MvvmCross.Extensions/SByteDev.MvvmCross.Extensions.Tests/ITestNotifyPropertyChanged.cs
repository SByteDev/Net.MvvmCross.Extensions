using System.ComponentModel;

namespace SByteDev.MvvmCross.Extensions.Tests
{
    // ReSharper disable once MemberCanBePrivate.Global
    public interface ITestNotifyPropertyChanged : INotifyPropertyChanged
    {
        string FirstProperty { get; }
        string SecondProperty { get; }
    }
}