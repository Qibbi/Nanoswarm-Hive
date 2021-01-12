using EasyHook;
using System;
using System.Runtime.InteropServices;

namespace Nanocore.RA3
{
    public static class ContainFix
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int SomethingDelegate(IntPtr hInstance, IntPtr hA1, IntPtr hA2, IntPtr hA3, IntPtr hA4);

        private static readonly SomethingDelegate _something = Marshal.GetDelegateForFunctionPointer<SomethingDelegate>(new IntPtr(0x00797CC0));

        private static int SomethingFunc(IntPtr hInstance, IntPtr hA1, IntPtr hA2, IntPtr hA3, IntPtr hA4)
        {
            if (hA3 == IntPtr.Zero)
            {
                return 2;// 0: dunno what 0 and 1 do differently, 2 enables you to have a forcefire cursor even out of the container's weapon range
            }
            return _something(hInstance, hA1, hA2, hA3, hA4);
        }

        public static void HookFix()
        {
            LocalHook somethingRange = LocalHook.Create(new IntPtr(0x00797CC0), new SomethingDelegate(SomethingFunc), null);
            somethingRange.ThreadACL.SetExclusiveACL(new[] { 0 });
        }
    }
}
