using Nanocore.Native;
using System;

namespace NanoswarmHive.Presentation.Windows
{
    public static class NativeHelper
    {
        public static void DisableMinimizeButton(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Invalid handle.");
            }
            User32.SetWindowLongA(handle, User32.GWL_STYLE, User32.GetWindowLongA(handle, User32.GWL_STYLE) & ~(int)User32.WindowStyle.MINIMIZEBOX);
        }
    }
}
