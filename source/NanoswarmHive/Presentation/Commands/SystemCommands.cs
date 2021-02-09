using NanoswarmHive.Presentation.Extensions;
using NanoswarmHive.Presentation.View;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace NanoswarmHive.Presentation.Commands
{
    public static class SystemCommands
    {
        private static readonly Lazy<ICommandBase> _lazyCloseWindowCommand = new Lazy<ICommandBase>(CloseWindowFactory);
        private static readonly Lazy<ICommandBase> _lazyMaximizeWindowCommand = new Lazy<ICommandBase>(MaximizeWindowFactory);
        private static readonly Lazy<ICommandBase> _lazyRestoreWindowCommand = new Lazy<ICommandBase>(RestoreWindowFactory);
        private static readonly Lazy<ICommandBase> _lazyMinimizeWindowCommand = new Lazy<ICommandBase>(MinimizeWindowFactory);
        private static readonly Lazy<ICommandBase> _lazyShowSystemMenuCommand = new Lazy<ICommandBase>(ShowSystemMenuFactory);

        public static ICommandBase CloseWindowCommand => _lazyCloseWindowCommand.Value;
        public static ICommandBase MaximizeWindowCommand => _lazyMaximizeWindowCommand.Value;
        public static ICommandBase RestoreWindowCommand => _lazyRestoreWindowCommand.Value;
        public static ICommandBase MinimizeWindowCommand => _lazyMinimizeWindowCommand.Value;
        public static ICommandBase ShowSystemMenuCommand => _lazyShowSystemMenuCommand.Value;

        static SystemCommands()
        {
        }

        private static bool HasFlag(Window window, Nanocore.Native.User32.WindowStyle flag)
        {
            return (Nanocore.Native.User32.GetWindowLongA(new WindowInteropHelper(window).Handle, Nanocore.Native.User32.GWL_STYLE) & (int)flag) != 0;
        }

        private static bool CanCloseWindow(Window window)
        {
            return true;
        }

        private static void CloseWindow(Window window)
        {
            System.Windows.SystemCommands.CloseWindow(window);
        }

        private static bool CanMaximizeWindow(Window window)
        {
            return HasFlag(window, Nanocore.Native.User32.WindowStyle.MAXIMIZEBOX);
        }

        private static void MaximizeWindow(Window window)
        {
            System.Windows.SystemCommands.MaximizeWindow(window);
        }

        private static bool CanRestoreWindow(Window window)
        {
            return HasFlag(window, Nanocore.Native.User32.WindowStyle.MAXIMIZEBOX);
        }

        private static void RestoreWindow(Window window)
        {
            System.Windows.SystemCommands.RestoreWindow(window);
        }

        private static bool CanMinimizeWindow(Window window)
        {
            return HasFlag(window, Nanocore.Native.User32.WindowStyle.MINIMIZEBOX);
        }

        private static void MinimizeWindow(Window window)
        {
            System.Windows.SystemCommands.MinimizeWindow(window);
        }

        private static bool CanShowSystemMenu(Window window)
        {
            return HasFlag(window, Nanocore.Native.User32.WindowStyle.SYSMENU);
        }

        private static void ShowSystemMenu(Window window)
        {
            ContentPresenter contentPresenter = window.FindVisualChildrenOfType<ContentPresenter>().FirstOrDefault(x => Equals(x.FindVisualParentOfType<Control>(), window));
            if (contentPresenter is null)
            {
                throw new InvalidOperationException("No content presenter present.");
            }
            System.Windows.SystemCommands.ShowSystemMenu(window, contentPresenter.PointToScreen(new Point(0.0, 0.0)));
        }

        private static ICommandBase CloseWindowFactory()
        {
            return new SystemCommand(new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) }),
                                     CanCloseWindow,
                                     CloseWindow);
        }

        private static ICommandBase MaximizeWindowFactory()
        {
            return new SystemCommand(new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) }),
                                     CanMaximizeWindow,
                                     MaximizeWindow);
        }

        private static ICommandBase RestoreWindowFactory()
        {
            return new SystemCommand(new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) }),
                                     CanRestoreWindow,
                                     RestoreWindow);
        }

        private static ICommandBase MinimizeWindowFactory()
        {
            return new SystemCommand(new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) }),
                                     CanMinimizeWindow,
                                     MinimizeWindow);
        }

        private static ICommandBase ShowSystemMenuFactory()
        {
            return new SystemCommand(new ViewModelServiceProvider(new[] { new DispatcherService(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher) }),
                                     CanShowSystemMenu,
                                     ShowSystemMenu);
        }
    }
}
