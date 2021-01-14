using System;
using System.Runtime.CompilerServices;

namespace Nanocore.Core.Serialization
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(IntPtr dest, IntPtr src, int count)
        {
            unsafe { Buffer.MemoryCopy((void*)src, (void*)dest, count, count); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(IntPtr dest, IntPtr src, long count)
        {
            unsafe { Buffer.MemoryCopy((void*)src, (void*)dest, count, count); }
        }
    }
}
