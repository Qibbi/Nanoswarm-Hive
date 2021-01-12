using System;
using System.Runtime.CompilerServices;

namespace Nanocore.Native
{
    public static class NativeLibrary
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Load(string libraryPath)
        {
            return Kernel32.Load(libraryPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetExport(IntPtr handle, string name)
        {
            return Kernel32.GetExport(handle, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Close(IntPtr handle)
        {
            return Kernel32.Close(handle);
        }
    }
}
