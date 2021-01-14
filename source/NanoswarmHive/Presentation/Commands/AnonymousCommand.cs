using Nanocore.Core.Extensions;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Commands
{
    public class AnonymousCommand : ACommandBase
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _action;

        public AnonymousCommand(IViewModelServiceProvider serviceProvider, Action action, Func<bool> canExecute = null) : base(serviceProvider)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            _action = x => action();
            _canExecute = canExecute;
        }

        public AnonymousCommand(IViewModelServiceProvider serviceProvider, Action<object> action, Func<bool> canExecute = null) : base(serviceProvider)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            bool result = base.CanExecute(parameter);
            return result && (_canExecute is null) ? result : _canExecute();
        }

        public override void Execute(object parameter)
        {
            _action(parameter);
        }
    }

    public class AnonymousTaskCommand : AnonymousCommand
    {
        public AnonymousTaskCommand(IViewModelServiceProvider serviceProvider, Func<Task> task, Func<bool> canExecute = null) : base(serviceProvider, x => task().Forget(), canExecute)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
        }
    }

    public class AnonymousCommand<T> : ACommandBase
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _action;

        public AnonymousCommand(IViewModelServiceProvider serviceProvider, Action<T> action, Func<T, bool> canExecute = null) : base(serviceProvider)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is null)
            {
                parameter = default;
            }
            if ((typeof(T).IsValueType || !(parameter is null)) && !(parameter is T)) throw new ArgumentException(nameof(parameter));
            bool result = base.CanExecute(parameter);
            return result && (_canExecute is null) ? result : _canExecute((T)parameter);
        }

        public override void Execute(object parameter)
        {
            if (parameter is null)
            {
                parameter = default;
            }
            if ((typeof(T).IsValueType || !(parameter is null)) && !(parameter is T)) throw new ArgumentException(nameof(parameter));
            _action((T)parameter);
        }
    }

    public class AnonymousTaskCommand<T> : AnonymousCommand<T>
    {
        public AnonymousTaskCommand(IViewModelServiceProvider serviceProvider, Func<T, Task> task, Func<T, bool> canExecute = null) : base(serviceProvider, x => task(x).Forget(), canExecute)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
        }
    }
}
