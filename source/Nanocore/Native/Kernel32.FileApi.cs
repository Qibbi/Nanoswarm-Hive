using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    partial class Kernel32
    {
        public const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]
        public struct FileTime
        {
            public uint DwLowDateTime;
            public uint DwHighDateTime;

            public long DateTime => ((long)DwHighDateTime << 32) | DwLowDateTime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Win32FindDataW
        {
            public int DwFileAttributes;
            public FileTime FtCreationTime;
            public FileTime FtLastAccessTime;
            public FileTime FtLastWriteTime;
            public uint NFileSizeHigh;
            public uint NFileSizeLow;
            public int DwReserved0;
            public int DwReserved1;
            public unsafe fixed char CFileName[MAX_PATH];
            public unsafe fixed char CAlternateFileName[14];
            public int DwFileType;
            public int DwCreatorType;
            public short WFinderFlags;

            public DateTime CreationTime => DateTime.FromFileTime(FtCreationTime.DateTime);
            public DateTime LastAccessTime => DateTime.FromFileTime(FtLastAccessTime.DateTime);
            public DateTime LastWriteTime => DateTime.FromFileTime(FtLastWriteTime.DateTime);
            public long FileSize => ((long)NFileSizeHigh << 32) | NFileSizeLow;
            public string FileName => GetFileName();
            public string AlternateFileName => GetAlternateFileName();

            private unsafe string GetFileName()
            {
                fixed (char* pFileName = CFileName)
                {
                    return Marshal.PtrToStringUni((IntPtr)pFileName);
                }
            }

            private unsafe string GetAlternateFileName()
            {
                fixed (char* pAlternateFileName = CAlternateFileName)
                {
                    return Marshal.PtrToStringUni((IntPtr)pAlternateFileName);
                }
            }
        }

        public delegate IntPtr FindFirstFileWDelegate([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName, ref Win32FindDataW lpFindFileData);
        public delegate bool FindCloseDelegate(IntPtr hFindFile);

        public static readonly FindFirstFileWDelegate FindFirstFileW;
        public static readonly FindCloseDelegate FindClose;
    }
}
