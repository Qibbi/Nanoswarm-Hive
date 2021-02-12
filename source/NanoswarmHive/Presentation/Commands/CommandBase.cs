using NanoswarmHive.Presentation.ViewModel;
using System;

namespace NanoswarmHive.Presentation.Commands
{
    public abstract class ACommandBase : ADispatcherViewModel, ICommandBase
    {
        private bool _isEnabled = true;

        public bool IsEnabled { get => _isEnabled; set => SetValue(ref _isEnabled, value, InvokeCanExecute); }

        public event EventHandler CanExecuteChanged;

        protected ACommandBase(IViewModelServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private void InvokeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseCanExecuteChanged(EventArgs args)
        {
            CanExecuteChanged?.Invoke(this, args);
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        public virtual bool CanExecute(object parameter)
        {
            return _isEnabled;
        }

        public void Execute()
        {
            Execute(null);
        }

        public abstract void Execute(object parameter);
    }
}
