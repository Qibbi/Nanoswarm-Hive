using Nanocore.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace NanoswarmHive.Presentation.Windows
{
    public sealed class WindowManager : IDisposable
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(WindowManager), "Prvides window management.");
        private static readonly string[] _debugWindowTypeNames = { "Microsoft.XamlDiagnostics.WpfTap", "Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerWindow" };
        private static readonly List<WindowInfo> _modalWindows = new List<WindowInfo>();
        private static readonly List<WindowInfo> _blockingWindows = new List<WindowInfo>();
        private static readonly HashSet<WindowInfo> _allWindows = new HashSet<WindowInfo>();

        private static Nanocore.Native.User32.WinEventProcDelegate _winEventProc;
        private static IntPtr _hook;
        private static Dispatcher _dispatcher;
        private static bool _isInitialized;

        public static WindowInfo MainWindow { get; private set; }
        public static IReadOnlyList<WindowInfo> BlockingWindows => _blockingWindows;

        public WindowManager(Dispatcher dispatcher)
        {
            if (_isInitialized) throw new InvalidOperationException($"Only one instance of {nameof(WindowManager)} may exist.");
            _isInitialized = true;
            _winEventProc = WinEventProc;
            _dispatcher = dispatcher;
            uint processId = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
            _hook = Nanocore.Native.User32.SetWinEventHook((uint)Nanocore.Native.User32.EventObject.SHOW,
                                                           (uint)Nanocore.Native.User32.EventObject.HIDE,
                                                           IntPtr.Zero,
                                                           _winEventProc,
                                                           processId,
                                                           0,
                                                           Nanocore.Native.User32.WinEventFlags.OutOfContext);
            if (_hook == IntPtr.Zero) throw new InvalidOperationException("Unable to initialize the window manager.");
            _tracer.TraceInfo($"{nameof(WindowManager)} initialized");
        }

        private static void ActivateMainWindow()
        {
            if (!(MainWindow is null) && MainWindow.HWnd != IntPtr.Zero)
            {
                Nanocore.Native.User32.SetActiveWindow(MainWindow.HWnd);
            }
        }

        private static void WindowShown(IntPtr hWnd)
        {
            if (!HWndHelper.HasStyleFlag(hWnd, Nanocore.Native.User32.WindowStyle.VISIBLE))
            {
                _tracer.TraceError("Discarding non-visible window ({0})", hWnd);
                return;
            }
            _tracer.TraceNote("Processing newly shown window ({0})...", hWnd);
            WindowInfo windowInfo = Find(hWnd);
            if (windowInfo is null)
            {
                windowInfo = new WindowInfo(hWnd);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    foreach (string debugWindowTypeName in _debugWindowTypeNames)
                    {
                        if (windowInfo.Window?.GetType().FullName.StartsWith(debugWindowTypeName) ?? false)
                        {
                            _tracer.TraceInfo("Discarding debug/diagnostics window '{0}' ({1})", windowInfo.Window.GetType().FullName, hWnd);
                            return;
                        }
                    }
                }
                _allWindows.Add(windowInfo);
            }
            windowInfo.IsShown = true;
            if (windowInfo == MainWindow)
            {
                _tracer.TraceInfo("Main window ({0}) shown.", hWnd);
                foreach (WindowInfo blockingWindow in _blockingWindows)
                {
                    _tracer.TraceInfo("Setting owner of existing blocking window ({0}) to be the main window ({1})", blockingWindow.HWnd, hWnd);
                    blockingWindow.Owner = MainWindow;
                }
                if (_modalWindows.Count > 0 || _blockingWindows.Count > 0)
                {
                    _tracer.TraceNote("Main window ({0}) disabled because a model or blocking window is already visible", MainWindow.HWnd);
                    MainWindow.IsDisabled = true;
                }
            }
            else if (windowInfo.IsBlocking)
            {
                _tracer.TraceInfo("Blocking window ({0}) shown", hWnd);
                if (!(MainWindow is null) && MainWindow.IsShown)
                {
                    _tracer.TraceInfo("Main window ({0}) disabled by new blocking window", MainWindow.HWnd);
                    MainWindow.IsDisabled = true;
                }
                if (_modalWindows.Count > 0)
                {
                    _tracer.TraceNote("Blocking window ({0}) disabled because a modal window is already visible", hWnd);
                    windowInfo.IsDisabled = true;
                }
                _blockingWindows.Add(windowInfo);
            }
            else if (windowInfo.IsModal)
            {
                _tracer.TraceInfo("Modal window ({0}) shown", hWnd);
                _modalWindows.Add(windowInfo);
            }
        }

        private static void WindowHidden(IntPtr hWnd)
        {
            _tracer.TraceNote("Processing newly hidden window ({0})...", hWnd);
            WindowInfo windowInfo = Find(hWnd);
            if (windowInfo is null)
            {
                windowInfo = new WindowInfo(hWnd);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    foreach (string debugWindowTypeName in _debugWindowTypeNames)
                    {
                        if (windowInfo.Window?.GetType().FullName.StartsWith(debugWindowTypeName) ?? false)
                        {
                            _tracer.TraceInfo("Discarding debug/diagnostics window '{0}' ({1})", windowInfo.Window.GetType().FullName, hWnd);
                            return;
                        }
                    }
                }
                _tracer.TraceError("This window was not handled by the {0} ({1})", nameof(WindowManager), hWnd);
                return;
            }
            windowInfo.IsShown = false;
            _allWindows.Remove(windowInfo);
            if (windowInfo == MainWindow)
            {
                _tracer.TraceNote("Main window ({0}) closed", hWnd);
                MainWindow = null;
            }
            else if (windowInfo.IsBlocking)
            {
                _tracer.TraceInfo("Blocking window ({0}) closed", hWnd);
                int idx = _blockingWindows.IndexOf(windowInfo);
                if (idx < 0) throw new InvalidOperationException("An unregistered blocking window has been closed");
                _blockingWindows.RemoveAt(idx);
                windowInfo.IsBlocking = false;
                if (!(MainWindow is null) && MainWindow.IsShown && _blockingWindows.Count == 0 && _modalWindows.Count == 0)
                {
                    _tracer.TraceNote("Main window ({0}) enabled because no more modal or blocking windows are visible", MainWindow.HWnd);
                    MainWindow.IsDisabled = false;
                }
                ActivateMainWindow();
            }
            else
            {
                _tracer.TraceInfo("Modal window ({0}) closed", hWnd);
                int idx = _modalWindows.IndexOf(windowInfo);
                if (idx >= 0)
                {
                    _modalWindows.RemoveAt(idx);
                    if (_modalWindows.Count == 0)
                    {
                        foreach (WindowInfo blockingWindow in _blockingWindows)
                        {
                            _tracer.TraceNote("Blocking window ({0}) enabled because no more modal windows are visible", blockingWindow.HWnd);
                            blockingWindow.IsDisabled = false;
                        }
                    }
                    if (!(MainWindow is null) && MainWindow.IsShown && _blockingWindows.Count == 0 && _modalWindows.Count == 0)
                    {
                        _tracer.TraceNote("Main window ({0}) enabled because no more modal or blocking windows are visible", MainWindow.HWnd);
                        MainWindow.IsDisabled = false;
                    }
                    ActivateMainWindow();
                }
            }
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint idEventThread, uint dwMsEventTime)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }
            IntPtr rootHWnd = Nanocore.Native.User32.GetAncestor(hWnd, Nanocore.Native.User32.GetAncestorFlags.Root);
            if (rootHWnd != IntPtr.Zero && rootHWnd != hWnd)
            {
                _tracer.TraceNote("Discarding non-root window ({0}) - root: ({1})", hWnd, rootHWnd);
            }
            if (eventType == (uint)Nanocore.Native.User32.EventObject.SHOW && idObject == 0)
            {
                if (_dispatcher.CheckAccess())
                {
                    WindowShown(hWnd);
                }
                else
                {
                    _dispatcher.InvokeAsync(() => WindowShown(hWnd));
                }
            }
            if (eventType == (uint)Nanocore.Native.User32.EventObject.HIDE && idObject == 0)
            {
                if (_dispatcher.CheckAccess())
                {
                    WindowHidden(hWnd);
                }
                else
                {
                    _dispatcher.InvokeAsync(() => WindowHidden(hWnd));
                }
            }
        }

        private static void CheckDispatcher()
        {
            if (_dispatcher.Thread != Thread.CurrentThread)
            {
                const string message = "Method must be invoked from dispatcher thread.";
                _tracer.TraceError(message);
                throw new InvalidOperationException(message);
            }
        }

        internal static WindowInfo Find(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return null;
            }
            WindowInfo result = _allWindows.FirstOrDefault(x => Equals(x.HWnd, hWnd));
            if (result is null)
            {
                Window window = WindowInfo.FromHWnd(hWnd);
                result = window is null ? null : _allWindows.FirstOrDefault(x => Equals(x.Window, window));
            }
            return result;
        }

        public static void ShowMainWindow(Window window)
        {
            CheckDispatcher();
            if (!(MainWindow is null))
            {
                const string message = "Main window is already set";
                _tracer.TraceError(message);
                throw new InvalidOperationException(message);
            }
            _tracer.TraceInfo("Main window showing ({0})", window);
            MainWindow = new WindowInfo(window);
            _allWindows.Add(MainWindow);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
        }

        public static void ShowBlockingWindow(Window window)
        {
            CheckDispatcher();
            WindowInfo windowInfo = new WindowInfo(window) { IsBlocking = true };
            if (_blockingWindows.Contains(windowInfo))
            {
                throw new InvalidOperationException("Window is already shown");
            }
            window.Owner = MainWindow?.Window;
            window.WindowStartupLocation = MainWindow is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
            if (!(MainWindow is null))
            {
                MainWindow.IsDisabled = true;
            }
            _allWindows.Add(windowInfo);
            _blockingWindows.Add(windowInfo);
            window.Loaded += (sender, args) => windowInfo.ForceHWndUpdate();
            window.Closed += (sender, args) => ActivateMainWindow();
            _tracer.TraceInfo("Modal window showing ({0})", window);
            window.Show();
        }

        public void Dispose()
        {
            if (!Nanocore.Native.User32.UnhookWinEvent(_hook)) throw new InvalidOperationException("An error occured while disposing the window manager.");
            _hook = IntPtr.Zero;
            _winEventProc = null;
            _dispatcher = null;
            MainWindow = null;
            _allWindows.Clear();
            _modalWindows.Clear();
            _blockingWindows.Clear();
            _tracer.TraceInfo("{0} disposed", nameof(WindowManager));
            _isInitialized = false;
        }
    }
}
