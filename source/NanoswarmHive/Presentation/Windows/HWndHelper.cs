using System;
using System.Runtime.CompilerServices;

namespace NanoswarmHive.Presentation.Windows
{
    public static class HWndHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool MatchFlag(int value, int flag)
        {
            return (value & flag) == flag;
        }

        internal static bool HasStyleFlag(IntPtr hWnd, Nanocore.Native.User32.WindowStyle flag)
        {
            int style = Nanocore.Native.User32.GetWindowLongA(hWnd, Nanocore.Native.User32.GWL_STYLE);
            return MatchFlag(style, (int)flag);
        }

        internal static bool HasExStyleFlag(IntPtr hWnd, Nanocore.Native.User32.WindowStyleEx flag)
        {
            int style = Nanocore.Native.User32.GetWindowLongA(hWnd, Nanocore.Native.User32.GWL_EXSTYLE);
            return MatchFlag(style, (int)flag);
        }

        public static bool IsDisabled(IntPtr hWnd)
        {
            return HasStyleFlag(hWnd, Nanocore.Native.User32.WindowStyle.DISABLED);
        }

        public static void SetDisabled(IntPtr hWnd, bool value)
        {
            int style = Nanocore.Native.User32.GetWindowLongA(hWnd, Nanocore.Native.User32.GWL_STYLE);
            if (value)
            {
                style |= (int)Nanocore.Native.User32.WindowStyle.DISABLED;
            }
            else
            {
                style &= ~(int)Nanocore.Native.User32.WindowStyle.DISABLED;
            }
            Nanocore.Native.User32.SetWindowLongA(hWnd, Nanocore.Native.User32.GWL_STYLE, style);
        }

        public static IntPtr GetOwner(IntPtr hWnd)
        {
            return Nanocore.Native.User32.GetWindow(hWnd, Nanocore.Native.User32.GW_OWNER);
        }

        public static void SetOwner(IntPtr hWnd, IntPtr ownerHWnd)
        {
            Nanocore.Native.User32.SetWindowLongA(hWnd, Nanocore.Native.User32.GWL_HWNDPARENT, (int)ownerHWnd);
        }
    }
}
