using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.View;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace NanoswarmHive.Presentation.Commands
{
    public static class UtilityCommands
    {
        private static readonly Lazy<ViewModelServiceProvider> _lazyServiceProvider = new Lazy<ViewModelServiceProvider>(ServiceProviderFactory);
        private static readonly Lazy<ICommandBase> _lazyDisabledCommand = new Lazy<ICommandBase>(DisabledCommandFactory);
        private static readonly Lazy<ICommandBase> _lazyOpenHyperlinkCommand = new Lazy<ICommandBase>(OpenHyperlinkCommandFactory);

        public static ICommandBase DisabledCommand => _lazyDisabledCommand.Value;
        public static ICommandBase OpenHyperlinkCommand
        {
            get
            {
                ICommandBase result = _lazyOpenHyperlinkCommand.Value;
                _lazyServiceProvider.Value.Get<IDispatcherService>().LowPriorityInvokeAsync(() => result.RaiseCanExecuteChanged(EventArgs.Empty));
                return result;
            }
        }

        private static bool CanOpenHyperlink(string url)
        {
            return !string.IsNullOrEmpty(url) && (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) || (new Uri(url).IsFile && File.Exists(url)));
        }

        private static void OpenHyperlink(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Win32Exception ex)
            {
                if (ex.ErrorCode == unchecked((int)0x80004005))
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static ViewModelServiceProvider ServiceProviderFactory()
        {
            return new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) });
        }

        private static ICommandBase DisabledCommandFactory()
        {
            return new AnonymousCommand(_lazyServiceProvider.Value, () => { }, () => false);
        }

        private static ICommandBase OpenHyperlinkCommandFactory()
        {
            return new AnonymousCommand<string>(_lazyServiceProvider.Value, OpenHyperlink, CanOpenHyperlink);
        }
    }
}
