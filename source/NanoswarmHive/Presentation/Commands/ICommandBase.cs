using System;
using System.Windows.Input;

namespace NanoswarmHive.Presentation.Commands
{
    public interface ICommandBase : ICommand
    {
        bool IsEnabled { get; set; }

        void Execute();

        void RaiseCanExecuteChanged(EventArgs args);
    }
}
