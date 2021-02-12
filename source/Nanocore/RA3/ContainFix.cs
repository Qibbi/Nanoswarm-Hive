using Nanocore.Core.Diagnostics;
using System;
using System.Runtime.InteropServices;

namespace Nanocore.RA3
{
    [Hook]
    public static class ContainFix
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(ContainFix), "Fixes range attack cursor for contained stuff");

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int SomethingDelegate(IntPtr hInstance, IntPtr hA1, IntPtr hA2, IntPtr hA3, IntPtr hA4);

        [HookFunction(nameof(SomethingFunc), Origin = 0x00797CC0, Steam = 0x00759680)]
        private static readonly SomethingDelegate _something;

        private static int SomethingFunc(IntPtr hInstance, IntPtr hA1, IntPtr hA2, IntPtr hA3, IntPtr hA4)
        {
            _tracer.TraceInfo("Checking if nullptr is passed");
            if (hA3 == IntPtr.Zero)
            {
                _tracer.TraceInfo("nullptr present, returning 2");
                return 2;// 0: dunno what 0 and 1 do differently, 2 enables you to have a forcefire cursor even out of the container's weapon range
            }
            return _something(hInstance, hA1, hA2, hA3, hA4);
        }
    }
}
