using NanoswarmHive.Presentation.Commands;
using NanoswarmHive.Presentation.Controls;
using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.View;
using NanoswarmHive.Presentation.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NanoswarmHive.Presentation.Windows
{
    public abstract class AMessageDialogBase : AModalWindow
    {
        private static readonly DependencyPropertyKey _buttonCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ButtonCommand), typeof(ICommandBase), typeof(AMessageDialogBase), new PropertyMetadata());

        public static readonly DependencyProperty ButtonComamndProperty = _buttonCommandPropertyKey.DependencyProperty;
        public static readonly DependencyProperty ButtonsSourceProperty =
            DependencyProperty.Register(nameof(ButtonsSource), typeof(IEnumerable<DialogButtonInfo>), typeof(AMessageDialogBase));
        public static readonly DependencyProperty MessageTemplateProperty =
            DependencyProperty.Register(nameof(MessageTemplate), typeof(DataTemplate), typeof(AMessageDialogBase));
        public static readonly DependencyProperty MessageTemplateSelectorProperty =
            DependencyProperty.Register(nameof(MessageTemplateSelector), typeof(DataTemplateSelector), typeof(AMessageDialogBase));
        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(nameof(Details), typeof(object), typeof(AMessageDialogBase));
        public static readonly DependencyProperty DetailsTemplateProperty =
            DependencyProperty.Register(nameof(DetailsTemplate), typeof(DataTemplate), typeof(AMessageDialogBase));
        public static readonly DependencyProperty DetailsTemplateSelectorProperty =
            DependencyProperty.Register(nameof(DetailsTemplateSelector), typeof(DataTemplateSelector), typeof(AMessageDialogBase));

        public ICommandBase ButtonCommand { get => (ICommandBase)GetValue(ButtonComamndProperty); private set => SetValue(_buttonCommandPropertyKey, value); }
        public IEnumerable<DialogButtonInfo> ButtonsSource { get => (IEnumerable<DialogButtonInfo>)GetValue(ButtonsSourceProperty); set => SetValue(ButtonsSourceProperty, value); }
        public DataTemplate MessageTemplate { get => (DataTemplate)GetValue(MessageTemplateProperty); set => SetValue(MessageTemplateProperty, value); }
        public DataTemplateSelector MessageTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(MessageTemplateSelectorProperty);
            set => SetValue(MessageTemplateSelectorProperty, value);
        }
        public string Details { get => (string)GetValue(DetailsProperty); set => SetValue(DetailsProperty, value); }
        public DataTemplate DetailsTemplate { get => (DataTemplate)GetValue(DetailsTemplateProperty); set => SetValue(DetailsTemplateProperty, value); }
        public DataTemplateSelector DetailsTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DetailsTemplateSelectorProperty);
            set => SetValue(DetailsTemplateSelectorProperty, value);
        }

        protected AMessageDialogBase()
        {
            ButtonCommand = new AnonymousCommand<DialogResultType>(new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher) }), ButtonClick);
        }

        private void ButtonClick(DialogResultType result)
        {
            Result = result;
            Close();
        }
    }
}
