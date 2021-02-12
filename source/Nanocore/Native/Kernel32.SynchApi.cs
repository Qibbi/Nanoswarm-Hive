using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public delegate IntPtr CreateEventADelegate(IntPtr lpEventAttributes,
                                                    [MarshalAs(UnmanagedType.Bool)] bool bManualReset,
                                                    [MarshalAs(UnmanagedType.Bool)] bool bInitialState,
                                                    [In, MarshalAs(UnmanagedType.LPTStr)] string lpName);
        public delegate int WaitForSingleObjectDelegate(IntPtr hHandle, int milliseconds);

        public static readonly CreateEventADelegate CreateEventA;
        public static readonly WaitForSingleObjectDelegate WaitForSingleObject;
    }
}
