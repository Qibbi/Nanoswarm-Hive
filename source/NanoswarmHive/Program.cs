using Microsoft.Win32;
using Nanocore;
using Nanocore.Core.Diagnostics;
using Nanocore.Native;
using Nanocore.Sage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Registry = Nanocore.Sage.Registry;

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

        [STAThread]
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
            string modConfig = null;
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
                    modConfig = args[idx++ + 1];
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
            bool isPathValid = false;
            string executablePath = string.Empty;
            if (!string.IsNullOrWhiteSpace(registry.InstallPath))
            {
                executablePath = System.IO.Path.Combine(registry.InstallPath, "data", "ra3_1.12.game");
                isPathValid = File.Exists(executablePath);
            }
            if (!isPathValid)
            {
                MessageBox.Show("Unable to locate a valid Red Alert 3 installation.\n\nPlease locate the game binary manually in the next window.");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "RA3 1.12 executable (ra3_1.12.game)|ra3_1.12.game|All files (*.*)|*.*";
                dialog.DefaultExt = "ra3_1.12.game";
                dialog.RestoreDirectory = true;
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    executablePath = dialog.FileName;
                    isPathValid = File.Exists(executablePath);
                }
                if (!isPathValid)
                {
                    MessageBox.Show("No valid Red Alert 3 binary specified.");
                    return -1;
                }
            }
            if (config == null)
            {
                isPathValid = false;
                if (!string.IsNullOrWhiteSpace(registry.InstallPath) && !string.IsNullOrWhiteSpace(registry.Language))
                {
                    config = System.IO.Path.Combine(registry.InstallPath, $"RA3_{registry.Language}_1.12.skudef");
                    isPathValid = File.Exists(config);
                }
                else
                {
                    MessageBox.Show("Unable to locate a valid Red Alert 3 config.\n\nPlease locate the config manually in the next window.");
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "RA3 config (*.skudef)|*.skudef|All files (*.*)|*.*";
                    dialog.DefaultExt = "*.skudef";
                    dialog.RestoreDirectory = true;
                    bool? result = dialog.ShowDialog();
                    if (result == true)
                    {
                        config = dialog.FileName;
                        isPathValid = File.Exists(config);
                    }
                    if (!isPathValid)
                    {
                        MessageBox.Show("No valid Red Alert 3 config specified.");
                        return -1;
                    }
                }
            }
            ExecutableType executableType = ExecutableType.Unknown;
            using (Stream stream = new FileStream(executablePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                uint hash = Nanocore.Core.FastHash.GetHashCode(buffer);
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
                MessageBox.Show("An unknown version of the game is installed. Please get the game from an official source.");
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
                    Kernel32.CreateProcessW(null, $"\"{executablePath}\" {string.Join(" ", args)} -config \"{config}\" {(modConfig is null ? string.Empty : $"-modconfig \"{modConfig}\"")}",
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
