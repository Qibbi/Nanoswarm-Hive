using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public delegate bool SetCurrentDirectoryWDelegate([In, MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

        public static readonly SetCurrentDirectoryWDelegate SetCurrentDirectoryW;
    }
}
