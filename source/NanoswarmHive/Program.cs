using Nanocore;
using Nanocore.Core.Diagnostics;
using Nanocore.Core.Language;
using Nanocore.Native;
using Nanocore.Sage;
using NanoswarmHive.Presentation.Dialogs;
using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.View;
using NanoswarmHive.Presentation.ViewModel;
using NanoswarmHive.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Threading;

namespace NanoswarmHive
{
    internal class Program
    {
        private sealed class InternalTraceWrite : IDisposable
        {
            public InternalTraceWrite(string source, TraceEventType eventType)
            {
                Console.Write($"[{DateTime.Now:HH:mm:ss:ffff}] [{source}] ");
                switch (eventType)
                {
                    case TraceEventType.Critical:
                    case TraceEventType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{eventType}: ");
                        break;
                    case TraceEventType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{eventType}: ");
                        break;
                    case TraceEventType.Information:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                }
            }

            public void Dispose()
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(Program), "Starts the game and initializes the hook.");
        private static bool _isConsoleCancel = false;
        private static string[] _args;
        private static App _app;

        private static void TraceWrite(string source, TraceEventType eventType, string message)
        {
            using (IDisposable disposable = new InternalTraceWrite(source, eventType))
            {
                Console.WriteLine(message);
            }
        }

        private static void ConsoleCancel(object sender, ConsoleCancelEventArgs args)
        {
            _isConsoleCancel = true;
            args.Cancel = true;
        }

        private static async void Startup()
        {
            bool useUI = false;
            for (int idx = 0; idx < _args.Length; ++idx)
            {
                if (string.Equals(_args[idx], "-ui"))
                {
                    useUI = true;
                }
            }
            Registry registry = new Registry();
            DispatcherService dispatcherService = new DispatcherService(Dispatcher.CurrentDispatcher);
            MessageBoxService messageBoxService = new MessageBoxService(dispatcherService, registry.DisplayName);
            string csfPath = Path.Combine(Environment.CurrentDirectory, "Launcher", $"{registry.Language}.csf");
            if (File.Exists(csfPath))
            {
                TranslationManager.Current.LoadStrings(csfPath);
            }
            else
            {
                await messageBoxService.MessageBox($"Language pack '{registry.Language}' is not installed.");
                _app.Shutdown(-1);
            }
            if (useUI)
            {
                ViewModelServiceProvider serviceProvider = new ViewModelServiceProvider(new List<object>
                {
                    dispatcherService,
                    messageBoxService,
                    registry
                });
                MainWindowViewModel mainViewModel = new MainWindowViewModel(serviceProvider);
                mainViewModel.LoadBackground();
                MainWindow mainWindow = new MainWindow(mainViewModel)
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                };
                mainWindow.ShowDialog();
                if (!mainViewModel.IsStartGame())
                {
                    _app.Shutdown();
                }
            }
            // TODO: use splash screen
            string config = null;
            string modconifg = null;
            List<string> argList = new List<string>(_args.Length);
            for (int idx = 0; idx < _args.Length; ++idx)
            {
                if (string.Equals(_args[idx], "-config"))
                {
                    if (idx == _args.Length - 1)
                    {
                        await messageBoxService.MessageBox("Invalid config parameter. A path needs to be set.");
                        _app.Shutdown(-1);
                    }
                    config = _args[idx++ + 1];
                }
                if (string.Equals(_args[idx], "-modconfig"))
                {
                    if (idx == _args.Length - 1)
                    {
                        await messageBoxService.MessageBox("Invalid modconfig parameter. A path needs to be set.");
                        _app.Shutdown(-1);
                    }
                    modconifg = _args[idx++ + 1];
                }
                else
                {
                    argList.Add(_args[idx]);
                }
            }
            _args = argList.ToArray();
            Kernel32.StartupInfoW si = new Kernel32.StartupInfoW(true);
            Kernel32.ProcessInformation pi = new Kernel32.ProcessInformation();
            int overallTries = 0;
            int tries = 0;
            string executablePath = System.IO.Path.Combine(registry.InstallPath, "data", "ra3_1.12.game");
            ExecutableType executableType = ExecutableType.Unknown;
            uint hash;
            using (System.IO.Stream stream = new System.IO.FileStream(executablePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                hash = Nanocore.Core.FastHash.GetHashCode(buffer);
                hash = 0xDEADBEEF;
                switch (hash)
                {
                    case 0xCFAAD44Bu:
                    case 0xE6D223E6u:
                    case 0xCF5817CCu:
                        executableType = ExecutableType.Steam;
                        break;
                    case 0xE7AF6A35u:
                    case 0x2F121290u:
                        executableType = ExecutableType.Origin;
                        break;
                    case 0xA05DEB39: // this has the 4gb thing applied, need to check the original
                        executableType = ExecutableType.Retail;
                        break;
                    case 0xBFE68CAD: // should I care? this is mostly here because origins and reloaded exe have the same size
                        executableType = ExecutableType.ReLOADeD;
                        break;
                }
            }
            if (executableType == ExecutableType.Unknown)
            {
                await messageBoxService.MessageBox($"An unknown version of the game is installed. Please get the game from an official source.\n\rIf your game is from an official source please [write me a message on Discord](https://discordapp.com/users/173165401864142858/) with this hash: {hash:X08}");
                _app.Shutdown(-1);
            }
            else if (executableType == ExecutableType.Retail)
            {
                await messageBoxService.MessageBox("The retail or an old origin version is installed. If you are using Origin please update the game. Retail versions cannot be supported due to SecuROM.");
                _app.Shutdown(-1);
            }
            while (!_isConsoleCancel)
            {
                ++tries;
                if (tries > 20)
                {
                    if (await messageBoxService.MessageBox("Windows caching interferred with injecting into the game, do you want to try again?", "Error", MessageBoxButtonType.YesNo) == MessageBoxResultType.Yes)
                    {
                        overallTries += tries;
                        tries = 1;
                    }
                    else
                    {
                        break;
                    }
                }
                try
                {
                    Kernel32.CreateProcessW(null, $"\"{executablePath}\" {string.Join(" ", _args)} -config \"{config ?? System.IO.Path.Combine(registry.InstallPath, $"RA3_{registry.Language}_1.12.skudef")}\" {(modconifg is null ? string.Empty : $"-modconfig \"{modconifg}\"")}",
                                            IntPtr.Zero,
                                            IntPtr.Zero,
                                            true,
                                            4,
                                            IntPtr.Zero,
                                            null,
                                            ref si,
                                            ref pi);
                    EZHook.Inject(pi.DwProcessId, "Nanocore.dll", pi.DwThreadId, executableType);
                }
                catch (ApplicationException ex)
                {
                    if ((uint)ex.HResult != 0x80131600u)
                    {
                        _tracer.TraceException(ex.Message);
                        await messageBoxService.MessageBox(ex.Message);
                        break;
                    }
                    _tracer.TraceInfo($"Failure attempt #{tries}. {ex.Message}");
                    Kernel32.TerminateProcess(pi.HProcess, -1);
                    Kernel32.ResumeThread(pi.HThread);
                    Kernel32.WaitForSingleObject(pi.HProcess, -1);
                    continue;
                }
                catch (Exception ex)
                {
                    _tracer.TraceException(ex.Message);
                    await messageBoxService.MessageBox(ex.Message);
                }
                break;
            }
            _app.Shutdown(overallTries + tries);
        }

        [STAThread]
        public static int Main(string[] args)
        {
            _args = args;
#if DEBUG
            IntPtr hConsole = IntPtr.Zero;
            if (Kernel32.AllocConsole())
            {
                hConsole = Kernel32.GetConsoleWindow();
                User32.SetLayeredWindowAttributes(hConsole, 0u, 225, 2);
            }
            User32.ShowWindow(hConsole, 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CancelKeyPress += ConsoleCancel;
#endif
            Tracer.TraceWrite += TraceWrite;
            Kernel32.Win32FindDataW findFileData = new Kernel32.Win32FindDataW();
            IntPtr hSearch = Kernel32.FindFirstFileW("lotrsec.big", ref findFileData);
            if (hSearch == (IntPtr)(-1))
            {
                StringBuilder fileName = new StringBuilder(Kernel32.MAX_PATH);
                Kernel32.GetModuleFileNameW(IntPtr.Zero, fileName, Kernel32.MAX_PATH);
                for (int idx = fileName.Length - 1; idx != 0; --idx)
                {
                    if (fileName[idx] == '\\')
                    {
                        fileName[idx] = '\0';
                        break;
                    }
                }
                Kernel32.SetCurrentDirectoryW(fileName.ToString());
            }
            else
            {
                Kernel32.FindClose(hSearch);
            }
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.InvokeAsync(Startup);
            using (new WindowManager(dispatcher))
            {
                _app = new App();
                _app.InitializeComponent();
                return _app.Run();
            }
        }
    }
}
