using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    internal static partial class User32
    {
        private const string _moduleName = "user32.dll";

        public static readonly IntPtr HModule;

        static User32()
        {
            HModule = NativeLibrary.Load(_moduleName);
            // Windows
            SetLayeredWindowAttributes = Marshal.GetDelegateForFunctionPointer<SetLayeredWindowAttributesDelegate>(NativeLibrary.GetExport(HModule, nameof(SetLayeredWindowAttributes)));
            ShowWindow = Marshal.GetDelegateForFunctionPointer<ShowWindowDelegate>(NativeLibrary.GetExport(HModule, nameof(ShowWindow)));
            GetWindow = Marshal.GetDelegateForFunctionPointer<GetWindowDelegate>(NativeLibrary.GetExport(HModule, nameof(GetWindow)));
            GetWindowLongA = Marshal.GetDelegateForFunctionPointer<GetWindowLongADelegate>(NativeLibrary.GetExport(HModule, nameof(GetWindowLongA)));
            SetWindowLongA = Marshal.GetDelegateForFunctionPointer<SetWindowLongADelegate>(NativeLibrary.GetExport(HModule, nameof(SetWindowLongA)));
            SetWinEventHook = Marshal.GetDelegateForFunctionPointer<SetWinEventHookDelegate>(NativeLibrary.GetExport(HModule, nameof(SetWinEventHook)));
            UnhookWinEvent = Marshal.GetDelegateForFunctionPointer<UnhookWinEventDelegate>(NativeLibrary.GetExport(HModule, nameof(UnhookWinEvent)));
            SetActiveWindow = Marshal.GetDelegateForFunctionPointer<SetActiveWindowDelegate>(NativeLibrary.GetExport(HModule, nameof(SetActiveWindow)));
            GetAncestor = Marshal.GetDelegateForFunctionPointer<GetAncestorDelegate>(NativeLibrary.GetExport(HModule, nameof(GetAncestor)));
        }
    }
}
