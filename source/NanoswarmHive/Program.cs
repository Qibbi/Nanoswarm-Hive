using Nanocore;
using Nanocore.Core.Diagnostics;
using Nanocore.Native;
using Nanocore.Sage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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

        public static int Main(string[] args)
        {
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
#endif
            Console.CancelKeyPress += ConsoleCancel;
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
            string config = null;
            string modconifg = null;
            List<string> argList = new List<string>(args.Length);
            for (int idx = 0; idx < args.Length; ++idx)
            {
                if (string.Equals(args[idx], "-config"))
                {
                    if (idx == args.Length - 1)
                    {
                        MessageBox.Show("Invalid config parameter. A path needs to be set.");
                        return -1;
                    }
                    config = args[idx++ + 1];
                }
                if (string.Equals(args[idx], "-modconfig"))
                {
                    if (idx == args.Length - 1)
                    {
                        MessageBox.Show("Invalid modconfig parameter. A path needs to be set.");
                        return -1;
                    }
                    modconifg = args[idx++ + 1];
                }
                else
                {
                    argList.Add(args[idx]);
                }
            }
            args = argList.ToArray();
            Kernel32.StartupInfoW si = new Kernel32.StartupInfoW(true);
            Kernel32.ProcessInformation pi = new Kernel32.ProcessInformation();
            int overallTries = 0;
            int tries = 0;
            Registry registry = new Registry();
            string executablePath = System.IO.Path.Combine(registry.InstallPath, "data", "ra3_1.12.game");
            long executableSize = new System.IO.FileInfo(executablePath).Length;
            ExecutableType executableType = ExecutableType.Unknown;
            switch (executableSize)
            {
                case 9658368:
                    executableType = ExecutableType.Steam;
                    break;
                case 9306112:
                    executableType = ExecutableType.Origin;
                    break;
                case 16504080:
                    executableType = ExecutableType.Retail;
                    break;
            }
            if (executableType == ExecutableType.Unknown)
            {
                MessageBox.Show("A version of the game is installed. Please get the game from an official source.");
                return -1;
            }
            else if (executableType == ExecutableType.Retail)
            {
                MessageBox.Show("The retail or an old origin version is installed. If you are using Origin please update the game. Retail versions cannot be supported due to SecuROM.");
                return -1;
            }
            while (!_isConsoleCancel)
            {
                ++tries;
                if (tries > 20)
                {
                    if (MessageBox.Show("Windows caching interferred with injecting into the game, do you want to try again?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                    Kernel32.CreateProcessW(null, $"\"{executablePath}\" {string.Join(" ", args)} -config \"{config ?? System.IO.Path.Combine(registry.InstallPath, $"RA3_{registry.Language}_1.12.skudef")}\" {(modconifg is null ? string.Empty : $"-modconfig \"{modconifg}\"")}",
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
                        MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                }
                break;
            }
            return overallTries + tries;
        }
    }
}
