using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    internal static partial class Ws2_32
    {
        private const string _moduleName = "ws2_32.dll";

        public static readonly IntPtr HModule;

        static Ws2_32()
        {
            HModule = NativeLibrary.Load(_moduleName);
            // WinSock
            GetHostByName = Marshal.GetDelegateForFunctionPointer<GetHostByNameDelegate>(NativeLibrary.GetExport(HModule, "gethostbyname"));
        }
    }
}
