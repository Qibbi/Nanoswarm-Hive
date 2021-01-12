using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    internal static partial class Kernel32
    {
        private const string _moduleName = "kernel32.dll";

        public static readonly IntPtr HModule;

        static Kernel32()
        {
            HModule = NativeLibrary.Load(_moduleName);
            // LibLoaderApi
            GetModuleFileNameW = Marshal.GetDelegateForFunctionPointer<GetModuleFileNameWDelegate>(NativeLibrary.GetExport(HModule, nameof(GetModuleFileNameW)));
            // ProcessThreadsApi
            CreateProcessW = Marshal.GetDelegateForFunctionPointer<CreateProcessWDelegate>(NativeLibrary.GetExport(HModule, nameof(CreateProcessW)));
            TerminateProcess = Marshal.GetDelegateForFunctionPointer<TerminateProcessDelegate>(NativeLibrary.GetExport(HModule, nameof(TerminateProcess)));
            OpenThread = Marshal.GetDelegateForFunctionPointer<OpenThreadDelegate>(NativeLibrary.GetExport(HModule, nameof(OpenThread)));
            ResumeThread = Marshal.GetDelegateForFunctionPointer<ResumeThreadDelegate>(NativeLibrary.GetExport(HModule, nameof(ResumeThread)));
            // SynchApi
            WaitForSingleObject = Marshal.GetDelegateForFunctionPointer<WaitForSingleObjectDelegate>(NativeLibrary.GetExport(HModule, nameof(WaitForSingleObject)));
            // MemoryApi
            VirtualProtect = Marshal.GetDelegateForFunctionPointer<VirtualProtectDelegate>(NativeLibrary.GetExport(HModule, nameof(VirtualProtect)));
            // HandleApi
            CloseHandle = Marshal.GetDelegateForFunctionPointer<CloseHandleDelegate>(NativeLibrary.GetExport(HModule, nameof(CloseHandle)));
            // FileApi
            FindFirstFileW = Marshal.GetDelegateForFunctionPointer<FindFirstFileWDelegate>(NativeLibrary.GetExport(HModule, nameof(FindFirstFileW)));
            FindClose = Marshal.GetDelegateForFunctionPointer<FindCloseDelegate>(NativeLibrary.GetExport(HModule, nameof(FindClose)));
            // WinBase
            SetCurrentDirectoryW = Marshal.GetDelegateForFunctionPointer<SetCurrentDirectoryWDelegate>(NativeLibrary.GetExport(HModule, nameof(SetCurrentDirectoryW)));
            // Windows
            AllocConsole = Marshal.GetDelegateForFunctionPointer<AllocConsoleDelegate>(NativeLibrary.GetExport(HModule, nameof(AllocConsole)));
            GetConsoleWindow = Marshal.GetDelegateForFunctionPointer<GetConsoleWindowDelegate>(NativeLibrary.GetExport(HModule, nameof(GetConsoleWindow)));
        }

        [DllImport(_moduleName, EntryPoint = "LoadLibraryA")] internal static extern IntPtr Load(string lpLibFileName);
        [DllImport(_moduleName, EntryPoint = "GetProcAddress")] internal static extern IntPtr GetExport(IntPtr hModule, string lpProcName);
        [DllImport(_moduleName, EntryPoint = "FreeLibrary")] internal static extern bool Close(IntPtr hLibModule);
    }
}
