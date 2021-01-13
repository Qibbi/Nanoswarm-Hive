using Nanocore.Core.Diagnostics;
using Nanocore.Native;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Nanocore
{
    public static class Steam
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(Steam), "Vital steam hook.");
        private static readonly byte[] _jump = new byte[] { 0xE9, 0x9B, 0x86, 0x79, 0xFF };

        public static unsafe bool ModifyEP(System.Diagnostics.Process process)
        {
            System.Diagnostics.ProcessModule module = process.MainModule;
            int oldProtection = 0;
            if (!Kernel32.VirtualProtect(module.BaseAddress, module.ModuleMemorySize, 0x40, ref oldProtection))
            {
                _tracer.TraceException(new Win32Exception(Marshal.GetLastWin32Error()).Message);
                return false;
            }
            byte* pCurrentModule = (byte*)module.BaseAddress.ToPointer();
            using (Stream stream = new UnmanagedMemoryStream(pCurrentModule, module.ModuleMemorySize, module.ModuleMemorySize, FileAccess.ReadWrite))
            {
                stream.Position = 0x0093b2ed;
                stream.Write(_jump, 0, _jump.Length);
            }
            return true;
        }
    }
}
