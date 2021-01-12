using EasyHook;
using Nanocore.Core.Diagnostics;
using Nanocore.Native;
using System;

namespace Nanocore
{
    public class Nanohook : IEntryPoint
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(Nanohook), "Main entry point for injection.");

        public Nanohook(RemoteHooking.IContext context, int threadId)
        {
#if DEBUG
            IntPtr consoleWindow = IntPtr.Zero;
            if (Kernel32.AllocConsole())
            {
                consoleWindow = Kernel32.GetConsoleWindow();
                User32.SetLayeredWindowAttributes(consoleWindow, 0u, 225, 2);
            }
            User32.ShowWindow(consoleWindow, 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "Nanocore Debug Console";
            Tracer.TraceWrite += Log;
            Tracer.SetTraceLevel(6);
#endif
        }

#if DEBUG
        private static void Log(string source, TraceEventType eventType, string message)
        {
            Console.Write(DateTime.Now.ToString("hh:mm:ss.fff "));
            ConsoleColor backupForeground;
            ConsoleColor backupBackground;
            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    backupForeground = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                    backupBackground = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write($"[{source}:{eventType}]");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = backupBackground;
                    Console.WriteLine(" " + message);
                    Console.ForegroundColor = backupForeground;
                    break;
                case TraceEventType.Warning:
                    backupForeground = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                    backupBackground = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.Write($"[{source}:{eventType}]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = backupBackground;
                    Console.WriteLine(" " + message);
                    Console.ForegroundColor = backupForeground;
                    break;
                default:
                    Console.WriteLine($"[{source}:{eventType}] {message}");
                    break;
            }
        }
#endif

        public void Run(RemoteHooking.IContext context, int threadId)
        {
            CncOnline.ModifyPublicKey(System.Diagnostics.Process.GetCurrentProcess());
            LocalHook getHostByName = LocalHook.Create(NativeLibrary.GetExport(Ws2_32.HModule, "gethostbyname"), new Ws2_32.GetHostByNameDelegate(CncOnline.GetHostByName), null);
            getHostByName.ThreadACL.SetExclusiveACL(new[] { 0 });

            RA3.ContainFix.HookFix();

            IntPtr thread = Kernel32.OpenThread(0x00100002, true, threadId);
            if (thread != IntPtr.Zero)
            {
                Kernel32.ResumeThread(thread);
                Kernel32.WaitForSingleObject(thread, -1);
                Kernel32.CloseHandle(thread);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
