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
        }
    }
}
