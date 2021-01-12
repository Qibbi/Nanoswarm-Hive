using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    internal static class MsVcRt
    {
        public delegate IntPtr ClearMemoryDelegate(IntPtr ptr, byte value, ulong count);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MemICmpDelegate([In] IntPtr buf1, [In] IntPtr buf2, SizeT size);

        private const string _moduleName = "msvcrt.dll";

        private static readonly IntPtr _hModule;

        private static readonly ClearMemoryDelegate _clearMemoryInt;

        public static readonly MemICmpDelegate MemICmp;

        static MsVcRt()
        {
            _hModule = NativeLibrary.Load(_moduleName);
            _clearMemoryInt = Marshal.GetDelegateForFunctionPointer<ClearMemoryDelegate>(NativeLibrary.GetExport(_hModule, "memset"));
            MemICmp = Marshal.GetDelegateForFunctionPointer<MemICmpDelegate>(NativeLibrary.GetExport(_hModule, "_memicmp"));
        }

        public static IntPtr ClearMemory(IntPtr ptr, byte value, int count)
        {
            return _clearMemoryInt(ptr, value, (ulong)count);
        }
    }
}
