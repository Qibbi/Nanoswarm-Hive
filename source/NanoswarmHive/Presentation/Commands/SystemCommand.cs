using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Windows;

namespace NanoswarmHive.Presentation.Commands
{
    internal class SystemCommand : ACommandBase
    {
        private readonly Func<Window, bool> _canExecute;
        private readonly Action<Window> _execute;

        internal SystemCommand(IViewModelServiceProvider serviceProvider, Func<Window, bool> canExecute, Action<Window> execute) : base(serviceProvider)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is Window window && _canExecute(window);
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is Window window))
            {
                throw new ArgumentException($"Illegal parameter, must be of type {nameof(Window)}.");
            }
            _execute(window);
        }
    }
}
