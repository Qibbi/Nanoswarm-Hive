using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    internal static partial class AdvApi32
    {
        private const string _moduleName = "advapi32.dll";

        public static readonly IntPtr HModule;

        static AdvApi32()
        {
            HModule = NativeLibrary.Load(_moduleName);
            // WinReg
            RegOpenKeyExW = Marshal.GetDelegateForFunctionPointer<RegOpenKeyExWDelegate>(NativeLibrary.GetExport(HModule, nameof(RegOpenKeyExW)));
            RegQueryValueExA = Marshal.GetDelegateForFunctionPointer<RegQueryValueExADelegate>(NativeLibrary.GetExport(HModule, nameof(RegQueryValueExA)));
            RegQueryValueExW = Marshal.GetDelegateForFunctionPointer<RegQueryValueExWDelegate>(NativeLibrary.GetExport(HModule, nameof(RegQueryValueExW)));
            RegCloseKey = Marshal.GetDelegateForFunctionPointer<RegCloseKeyDelegate>(NativeLibrary.GetExport(HModule, nameof(RegCloseKey)));
        }
    }
}
