using System;

namespace Nanocore.Native
{
    partial class User32
    {
        public delegate bool SetLayeredWindowAttributesDelegate(IntPtr hWnd, uint crKey, byte bAlpha, int dwFlags);
        public delegate bool ShowWindowDelegate(IntPtr hWnd, int nCmdShow);

        public static readonly SetLayeredWindowAttributesDelegate SetLayeredWindowAttributes;
        public static readonly ShowWindowDelegate ShowWindow;
    }
}
