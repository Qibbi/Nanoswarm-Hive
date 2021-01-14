using EasyHook;
using Nanocore.Core.Diagnostics;
using Nanocore.Native;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Nanocore
{
    public class Nanohook : IEntryPoint
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(Nanohook), "Main entry point for injection.");

        public Nanohook(RemoteHooking.IContext context, int threadId, ExecutableType executableType)
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
#endif
            Tracer.TraceWrite += Log;
            Tracer.SetTraceLevel(6);
        }

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

        private void InitializeHooks(ExecutableType executableType)
        {
            // TODO: this is a perfect thing for code generation, but that's still in preview
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.DefinedTypes)
            {
                if (type.GetCustomAttribute<HookAttribute>() is null)
                {
                    continue;
                }
                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (fieldInfo.GetCustomAttribute<HookFunctionAttribute>() is HookFunctionAttribute hookFunctionAttribute)
                    {
                        IntPtr fnPtr = IntPtr.Zero;
                        switch (executableType)
                        {
                            case ExecutableType.Steam:
                            case ExecutableType.ReLOADeD:
                                fnPtr = new IntPtr(hookFunctionAttribute.Steam);
                                break;
                            case ExecutableType.Origin:
                                fnPtr = new IntPtr(hookFunctionAttribute.Origin);
                                break;
                        }
                        if (fnPtr == IntPtr.Zero)
                        {
                            _tracer.TraceWarning("Hook {0}.{1} doesn't have a valid function pointer for the executable, hook not enabled.", type.Name, hookFunctionAttribute.FunctionName);
                            continue;
                        }
                        fieldInfo.SetValue(null, Marshal.GetDelegateForFunctionPointer(fnPtr, fieldInfo.FieldType));
                        MethodInfo fn = null;
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (method.Name.Equals(hookFunctionAttribute.FunctionName)) // TODO: find a better way to check which funciton to use
                            {
                                fn = method;
                                break;
                            }
                        }
                        if (fn is null)
                        {
                            _tracer.TraceWarning("Hook {0}.{1} cannot find the function, hook not enabled.", type.Name, hookFunctionAttribute.FunctionName);
                            continue;
                        }
                        LocalHook hook = LocalHook.Create(fnPtr, Delegate.CreateDelegate(fieldInfo.FieldType, fn), null);
                        hook.ThreadACL.SetExclusiveACL(new[] { 0 });
                        _tracer.TraceNote("Hook {0}.{1} enabled.", type.Name, hookFunctionAttribute.FunctionName);
                    }
                    else if (fieldInfo.GetCustomAttribute<GameFunctionAttribute>() is GameFunctionAttribute gameFunctionAttribute)
                    {
                        IntPtr fnPtr = IntPtr.Zero;
                        switch (executableType)
                        {
                            case ExecutableType.Steam:
                            case ExecutableType.ReLOADeD:
                                fnPtr = new IntPtr(gameFunctionAttribute.Steam);
                                break;
                            case ExecutableType.Origin:
                                fnPtr = new IntPtr(gameFunctionAttribute.Origin);
                                break;
                        }
                        if (fnPtr == IntPtr.Zero)
                        {
                            _tracer.TraceError("Function {0}.{1} doesn't have a valid function pointer for the executable, this will cause a crash if called.", type.Name, fieldInfo.Name);
                            continue;
                        }
                        fieldInfo.SetValue(null, Marshal.GetDelegateForFunctionPointer(fnPtr, fieldInfo.FieldType));
                    }
                    else if (fieldInfo.GetCustomAttribute<GamePointerAttribute>() is GamePointerAttribute gamePointerAttribute)
                    {
                        IntPtr fnPtr = IntPtr.Zero;
                        switch (executableType)
                        {
                            case ExecutableType.Steam:
                            case ExecutableType.ReLOADeD:
                                fnPtr = new IntPtr(gamePointerAttribute.Steam);
                                break;
                            case ExecutableType.Origin:
                                fnPtr = new IntPtr(gamePointerAttribute.Origin);
                                break;
                        }
                        if (fnPtr == IntPtr.Zero)
                        {
                            _tracer.TraceError("Pointer {0}.{1} doesn't have a valid pointer for the executable, this will cause a crash if dereferenced.", type.Name, fieldInfo.Name);
                            continue;
                        }
                        fieldInfo.SetValue(null, fnPtr);
                    }
                }
            }
        }

        public void Run(RemoteHooking.IContext context, int threadId, ExecutableType exeType)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            _tracer.TraceNote("Trying to hook executable detected as '{0}'.", exeType);
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            if (exeType == ExecutableType.Steam)
            {
                Steam.ModifyEP(process);
            }
            CncOnline.ModifyPublicKey(process);
            LocalHook getHostByName = LocalHook.Create(NativeLibrary.GetExport(Ws2_32.HModule, "gethostbyname"), new Ws2_32.GetHostByNameDelegate(CncOnline.GetHostByName), null);
            getHostByName.ThreadACL.SetExclusiveACL(new[] { 0 });
            LocalHook send = LocalHook.Create(NativeLibrary.GetExport(Ws2_32.HModule, "send"), new Ws2_32.SendDelegate(CncOnline.Send), null);
            send.ThreadACL.SetExclusiveACL(new[] { 0 });

            InitializeHooks(exeType);

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
