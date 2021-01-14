using NanoswarmHive.Presentation.Commands;
using NanoswarmHive.Presentation.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace NanoswarmHive.Presentation.Controls
{
    public class WindowButtonInfo : AViewModelBase
    {
        private ICommandBase _command;
        private object _commandParameter;
        private object _content;
        private object _toolTip;

        public ICommandBase Command { get => _command; set => SetValue(ref _command, value); }
        public object CommandParameter { get => _commandParameter; set => SetValue(ref _commandParameter, value); }
        public object Content { get => _content; set => SetValue(ref _content, value); }
        public object ToolTip { get => _toolTip; set => SetValue(ref _toolTip, value); }
    }

    public class WindowButtonFreeSpaceInfo : AViewModelBase
    {
        public static readonly WindowButtonFreeSpaceInfo Default = new WindowButtonFreeSpaceInfo();
    }

    public class WindowButtonTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WindowButtonInfo)
            {
                return (container as FrameworkElement).FindResource("WindowButtonTemplate") as DataTemplate;
            }
            else if (item is WindowButtonFreeSpaceInfo)
            {
                return (container as FrameworkElement).FindResource("WindowButtonFreeSpaceTemplate") as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
