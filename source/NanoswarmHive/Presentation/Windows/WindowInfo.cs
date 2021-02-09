using System;
using System.Windows;
using System.Windows.Interop;

namespace NanoswarmHive.Presentation.Windows
{
    public class WindowInfo : IEquatable<WindowInfo>
    {
        private IntPtr _hWnd;
        private bool _isShown;

        public Window Window { get; }
        public IntPtr HWnd => _hWnd == IntPtr.Zero && !(Window is null) ? ToHWnd(Window) : _hWnd;
        public bool IsDisabled { get => HWndHelper.IsDisabled(_hWnd); internal set => HWndHelper.SetDisabled(_hWnd, value); }
        public bool IsShown
        {
            get => _isShown;
            internal set
            {
                _isShown = false;
                ForceHWndUpdate();
            }
        }
        public bool IsBlocking { get; internal set; }
        public WindowInfo Owner
        {
            get
            {
                if (!IsShown)
                {
                    return null;
                }
                if (!(Window?.Owner is null))
                {
                    return WindowManager.Find(ToHWnd(Window.Owner));
                }
                IntPtr owner = HWndHelper.GetOwner(HWnd);
                return owner != IntPtr.Zero ? WindowManager.Find(owner) ?? new WindowInfo(owner) : null;
            }
            internal set
            {
                if (value == Owner)
                {
                    return;
                }
                if (!(Window is null))
                {
                    if (value?.Window is null)
                    {
                        Window.Owner = null;
                        if (!(value is null))
                        {
                            HWndHelper.SetOwner(HWnd, value.HWnd);
                        }
                    }
                    else
                    {
                        Window.Owner = value.Window;
                    }
                }
                else
                {
                    HWndHelper.SetOwner(HWnd, value?.HWnd ?? IntPtr.Zero);
                }
            }
        }
        public bool IsModal
        {
            get
            {
                if (IsBlocking ||
                    HWnd == IntPtr.Zero ||
                    HWndHelper.HasExStyleFlag(HWnd, Nanocore.Native.User32.WindowStyleEx.TOOLWINDOW) ||
                    HWndHelper.HasStyleFlag(HWnd, Nanocore.Native.User32.WindowStyle.CHILD))
                {
                    return false;
                }
                WindowInfo owner = Owner;
                return owner is null || (owner.IsModal && owner.IsDisabled);
            }
        }

        internal WindowInfo(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException(nameof(hWnd));
            Window window = FromHWnd(hWnd);
            Window = window;
            if (Window is null)
            {
                _hWnd = hWnd;
            }
        }

        public WindowInfo(Window window)
        {
            Window = window;
        }

        public static bool operator ==(WindowInfo x, WindowInfo y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(WindowInfo x, WindowInfo y)
        {
            return !x.Equals(y);
        }

        internal static Window FromHWnd(IntPtr hWnd)
        {
            return hWnd != IntPtr.Zero ? HwndSource.FromHwnd(hWnd)?.RootVisual as Window : null;
        }

        internal static IntPtr ToHWnd(Window window)
        {
            return !(window is null) ? new WindowInteropHelper(window).Handle : IntPtr.Zero;
        }

        internal void ForceHWndUpdate()
        {
            if (!(Window is null))
            {
                _hWnd = ToHWnd(Window);
            }
        }

        public bool Equals(WindowInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Window, other.Window) && Equals(HWnd, other.HWnd);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WindowInfo);
        }

        public override int GetHashCode()
        {
            return Window?.GetHashCode() ?? 0;
        }
    }
}
