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
using System.Threading.Tasks;
using System.Windows;
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
        private static string[] _args;
        private static App _app;

        private static void TraceWrite(string source, TraceEventType eventType, string message)
        {
            using (IDisposable disposable = new InternalTraceWrite(source, eventType))
            {
                Console.WriteLine(message);
            }
        }

        private static async Task Startup()
        {
            bool useUI = false;
            for (int idx = 0; idx < _args.Length; ++idx)
            {
                if (string.Equals(_args[idx], "-ui"))
                {
                    useUI = true;
                }
            }
            useUI = true;
            Registry registry = new Registry
            {
                DisplayName = "Command & Conquer Generals Evolution"
            };
            DispatcherService dispatcherService = new DispatcherService(Dispatcher.CurrentDispatcher);
            MessageBoxService messageBoxService = new MessageBoxService(dispatcherService, registry.DisplayName);
            string csfPath = Path.Combine(Environment.CurrentDirectory, "Launcher", $"{registry.Language}.csf");
            if (File.Exists(csfPath))
            {
                TranslationManager.Current.LoadStrings(csfPath);
            }
            else
            {
                csfPath = Path.Combine(Environment.CurrentDirectory, "Launcher", "english.csf");
                if (File.Exists(csfPath))
                {
                    TranslationManager.Current.LoadStrings(csfPath);
                }
                else
                {
                    await messageBoxService.MessageBox($"Language pack '{registry.Language}' is not installed.");
                    _app.Shutdown(-1);
                    return;
                }
            }
            ViewModelServiceProvider serviceProvider = new ViewModelServiceProvider(new List<object>
                {
                    dispatcherService,
                    messageBoxService,
                    registry
                });
            if (useUI)
            {
                MainWindowViewModel mainViewModel = new MainWindowViewModel(serviceProvider);
                mainViewModel.LoadBackground();
                MainWindow mainWindow = new MainWindow(mainViewModel)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                Application.Current.MainWindow = mainWindow;
                WindowManager.ShowMainWindow(mainWindow);
            }
            else
            {
                await Launch(serviceProvider, false);
            }
        }

        private static void CopyCursors(string source, string destination)
        {
            DirectoryInfo dInfo = Directory.CreateDirectory(destination);
            foreach (string str in Directory.GetFiles(source))
            {
                File.Copy(str, Path.Combine(dInfo.FullName, Path.GetFileName(str)));
            }
        }

        public static async Task Launch(IViewModelServiceProvider serviceProvider, bool useGui)
        {
            MessageBoxService messageBoxService = serviceProvider.Get<MessageBoxService>();
            Registry registry = serviceProvider.Get<Registry>();
            SplashScreenViewModel splashViewModel = new SplashScreenViewModel(serviceProvider);
            splashViewModel.LoadSplash();
            SplashScreen splashScreen = new SplashScreen(splashViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            WindowManager.ShowBlockingWindow(splashScreen);
            string config = null;
            string modconfig = null;
            List<string> argList = new List<string>(_args.Length);
            for (int idx = 0; idx < _args.Length; ++idx)
            {
                if (string.Equals(_args[idx], "-config"))
                {
                    if (idx == _args.Length - 1)
                    {
                        await messageBoxService.MessageBox("Invalid config parameter. A path needs to be set.");
                        _app.Shutdown(-1);
                        return;
                    }
                    config = _args[idx++ + 1];
                }
                if (string.Equals(_args[idx], "-modconfig"))
                {
                    if (idx == _args.Length - 1)
                    {
                        await messageBoxService.MessageBox("Invalid modconfig parameter. A path needs to be set.");
                        _app.Shutdown(-1);
                        return;
                    }
                    modconfig = _args[idx++ + 1];
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
            string executablePath = Path.Combine(registry.InstallPath, "data", "ra3_1.12.game");
            string dataPath = Path.Combine(Environment.CurrentDirectory, "data");
            ExecutableType executableType = ExecutableType.Unknown;
            uint hash;
            using (Stream stream = new FileStream(executablePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                hash = Nanocore.Core.FastHash.GetHashCode(buffer);
                switch (hash)
                {
                    case 0xCFAAD44Bu:
                    case 0xE6D223E6u:
                    case 0xCF5817CCu:
                    case 0xA5D6B4D8u:
                    case 0xCC7D2897u:
                    case 0xE92FD5BCu:
                        executableType = ExecutableType.Steam;
                        break;
                    case 0xE7AF6A35u:
                    case 0x2F121290u:
                    case 0x15A0610Du:
                        executableType = ExecutableType.Origin;
                        break;
                    case 0x75DA0A02u: // Origin with activation
                        try
                        {
                            if (!File.Exists(Path.Combine(dataPath, "ra3_1.12.game")))
                            {
                                byte[] xome = File.ReadAllBytes(Path.Combine(dataPath, "xome.o.dat"));
                                for (int idx = 0; idx < xome.Length; ++idx)
                                {
                                    buffer[idx] = (byte)(buffer[idx] ^ xome[idx]);
                                }
                                using (Stream ostream = new FileStream(Path.Combine(dataPath, "ra3_1.12.game"), FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    ostream.Write(buffer, 0, xome.Length);
                                }
                                CopyCursors(Path.Combine(Path.GetDirectoryName(executablePath), "data", "cursors"), Path.Combine(dataPath, "Data", "Cursors"));
                            }
                        }
                        catch
                        {
                            await messageBoxService.MessageBox($"Unable to access filesystem. Please launch in administrator mode (has to be done only once), or use a directory without special priviledges.");
                            _app.Shutdown(-1);
                            return;
                        }
                        executablePath = Path.Combine(dataPath, "ra3_1.12.game");
                        executableType = ExecutableType.Origin;
                        break;
                    case 0x13F3E041u:
                        try
                        {
                            if (!File.Exists(Path.Combine(dataPath, "ra3_1.12.game")))
                            {
                                byte[] xome = File.ReadAllBytes(Path.Combine(dataPath, "xome.o.dat"));
                                for (int idx = 0; idx < xome.Length; ++idx)
                                {
                                    buffer[idx] = (byte)(buffer[idx] ^ xome[idx]);
                                }
                                buffer[0x168] = 0x4F;
                                buffer[0x169] = 0x2A;
                                using (Stream ostream = new FileStream(Path.Combine(dataPath, "ra3_1.12.game"), FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    ostream.Write(buffer, 0, xome.Length);
                                }
                                CopyCursors(Path.Combine(Path.GetDirectoryName(executablePath), "data", "cursors"), Path.Combine(dataPath, "Data", "Cursors"));
                            }
                        }
                        catch
                        {
                            await messageBoxService.MessageBox($"Unable to access filesystem. Please launch in administrator mode (has to be done only once), or use a directory without special priviledges.");
                            _app.Shutdown(-1);
                            return;
                        }
                        executablePath = Path.Combine(dataPath, "ra3_1.12.game");
                        executableType = ExecutableType.Origin;
                        break;
                    case 0x2187AF73u:
                    case 0xA05DEB39u:
                        executableType = ExecutableType.Retail;
                        break;
                    case 0xBFE68CADu: // should I care? this is mostly here because origins and reloaded exe have the same size, not just reloaded, seems to be repackaged by others
                    case 0xD55E5467u:
                    case 0xD3D7E1F7u:
                        executableType = ExecutableType.ReLOADeD;
                        break;
                }
            }
            if (executableType == ExecutableType.Unknown)
            {
                await messageBoxService.MessageBox($"An unknown version of the game is installed. Please get the game from an official source.\n\rIf your game is from an official source please write a message on [the WrathEd/Nanoswarm Discord](https://discord.gg/BcE3HB9W6e) with this hash: {hash:X08}");
                _app.Shutdown(-1);
                return;
            }
            else if (executableType == ExecutableType.OriginActivation)
            {
                await messageBoxService.MessageBox("The origin version installed requires Origin Authentication and is not yet supported. For now you can use the alternate installation and launch method to play.");
                _app.Shutdown(-1);
                return;
            }
            else if (executableType == ExecutableType.Retail)
            {
                await messageBoxService.MessageBox("The retail or an old origin version is installed. If you are using Origin please update the game. Retail versions cannot be supported due to SecuROM, thus you have to use the alternate installation and launch method to play.");
                // await messageBoxService.MessageBox("The retail or an old origin version is installed. If you are using Origin please update the game. Retail versions cannot be supported due to SecuROM.");
                _app.Shutdown(-1);
                return;
            }
            _tracer.TraceInfo("Launching {0} executable", executableType.ToString());
            // IntPtr hCloseSplash = Kernel32.CreateEventA(IntPtr.Zero, false, false, "LauncherCloseSplashscreen");
            DateTime now = DateTime.Now;
            while (true)
            {
                ++tries;
                _tracer.TraceInfo($"Attempt #{tries}");
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
                    Kernel32.CreateProcessW(null, $"\"{executablePath}\" {string.Join(" ", _args)} -config \"{config ?? Path.Combine(registry.InstallPath, $"RA3_{registry.Language}_1.12.skudef")}\" -modconfig \"{modconfig ?? Path.Combine(Environment.CurrentDirectory, "GenEvo_B0.1.skudef")}\"",
                                            IntPtr.Zero,
                                            IntPtr.Zero,
                                            true,
                                            4,
                                            IntPtr.Zero,
                                            null,
                                            ref si,
                                            ref pi);
                    _tracer.TraceInfo("Process created");
                    System.Threading.Thread.MemoryBarrier();
                    EZHook.Inject(pi.DwProcessId, "Nanocore.dll", pi.DwThreadId, executableType);
                    _tracer.TraceInfo("Process injected");
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
                    _tracer.TraceInfo("Process terminated, retrying");
                    continue;
                }
                catch (Exception ex)
                {
                    _tracer.TraceException(ex.Message);
                    await messageBoxService.MessageBox(ex.Message);
                }
                break;
            }
            while (true)
            {
                if ((DateTime.Now - now).TotalMilliseconds > 5000)
                {
                    splashScreen.Close();
                    break;
                }
                // int waitResult = Kernel32.WaitForSingleObject(hCloseSplash, 500);
                // if (waitResult == 0)
                // {
                //     splashScreen.Close();
                //     break;
                // }
                await Task.Delay(50);
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
            Tracer.TraceWrite += TraceWrite;
            Tracer.SetTraceLevel(6);
#endif
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
