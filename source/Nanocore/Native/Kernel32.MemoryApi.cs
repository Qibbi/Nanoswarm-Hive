using System;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public delegate bool VirtualProtectDelegate(IntPtr lpAddress, int dwSize, int flNewProtect, ref int lpflOldProtect);

        public static readonly VirtualProtectDelegate VirtualProtect;
    }
}
