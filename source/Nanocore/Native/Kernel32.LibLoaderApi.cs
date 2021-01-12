using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public delegate int GetModuleFileNameWDelegate(IntPtr hModule, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpFileName, int nSize);

        public static readonly GetModuleFileNameWDelegate GetModuleFileNameW;
    }
}
