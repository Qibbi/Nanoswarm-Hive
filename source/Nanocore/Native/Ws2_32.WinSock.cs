using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    partial class Ws2_32
    {
        public delegate IntPtr GetHostByNameDelegate([In] string name);
        public delegate int SendDelegate(IntPtr s, IntPtr buf, int len, int flags);

        public static GetHostByNameDelegate GetHostByName;
        public static SendDelegate Send;
    }
}
