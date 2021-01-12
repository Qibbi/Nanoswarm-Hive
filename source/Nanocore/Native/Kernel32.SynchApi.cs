using System;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public delegate int WaitForSingleObjectDelegate(IntPtr hHandle, int milliseconds);

        public static readonly WaitForSingleObjectDelegate WaitForSingleObject;
    }
}
