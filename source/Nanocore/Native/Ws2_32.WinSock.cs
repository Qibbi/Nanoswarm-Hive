using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    partial class Ws2_32
    {
        public delegate IntPtr GetHostByNameDelegate([In] string name);

        public static GetHostByNameDelegate GetHostByName;
    }
}
