using Nanocore.Native;
using System;
using System.Runtime.InteropServices;

namespace Nanocore.Sage
{
    public static class MemoryPool
    {
        public static bool CanChangeZeroFillMemory { get; set; }
        public static bool ZeroFillMemory { get; private set; }

        private static unsafe IntPtr AllocateMemory(int sizeInBytes, int align = 16)
        {
            ulong mask = (ulong)(align - 1);
            if ((align & (int)mask) != 0) throw new ArgumentException("Not power of two", nameof(align));
            IntPtr ptr = Marshal.AllocHGlobal(sizeInBytes + (int)mask + sizeof(void*));
            byte* result = (byte*)(((ulong)ptr + (ulong)sizeof(void*) + mask) & ~mask);
            ((IntPtr*)result)[-1] = ptr;
            return new IntPtr(result);
        }

        private static unsafe IntPtr AllocateClearedMemory(int sizeInBytes, byte value = 0, int align = 16)
        {
            IntPtr result = AllocateMemory(sizeInBytes, align);
            MsVcRt.ClearMemory(result, value, sizeInBytes);
            return result;
        }

        public static IntPtr Allocate(int size, int count)
        {
            IntPtr result;
            if (ZeroFillMemory)
            {
                result = AllocateClearedMemory(size * count, 0);
            }
            else
            {
                result = AllocateMemory(size * count);
            }
            return result;
        }

        public static IntPtr Allocate(int size)
        {
            return Allocate(size, 1);
        }

        public static unsafe void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(((IntPtr*)ptr)[-1]);
        }

        public static void UnsetZeroFillMemory()
        {
            if (CanChangeZeroFillMemory)
            {
                ZeroFillMemory = false;
            }
        }

        public static void SetZeroFillMemory()
        {
            if (CanChangeZeroFillMemory)
            {
                ZeroFillMemory = true;
            }
        }
    }
}
