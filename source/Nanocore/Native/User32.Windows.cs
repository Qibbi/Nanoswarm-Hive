using System;

namespace Nanocore.Native
{
    partial class User32
    {
        [Flags]
        public enum WindowStyle : uint
        {
            OVERLAPPED = 0x00000000,
            TILED = OVERLAPPED,
            TABSTOP = 0x00010000,
            MAXIMIZEBOX = 0x00010000,
            MINIMIZEBOX = 0x00020000,
            SIZEBOX = 0x00040000,
            THICKFRAME = SIZEBOX,
            SYSMENU = 0x00080000,
            HSCROLL = 0x00100000,
            VSCROLL = 0x00200000,
            GROUP = 0x00200000,
            DLGFRAME = 0x00400000,
            BORDER = 0x00800000,
            CAPTION = 0x00C00000,
            MAXIMIZE = 0x01000000,
            CLIPCHILDREN = 0x02000000,
            CLIPSIBLINGS = 0x04000000,
            DISABLED = 0x08000000,
            VISIBLE = 0x10000000,
            ICONIC = 0x20000000,
            MINIMIZE = ICONIC,
            CHILD = 0x40000000,
            CHILDWINDOW = CHILD,
            POPUP = 0x80000000,
            OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
            POPUPWINDOW = POPUP | BORDER | SYSMENU,
            TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX
        }

        [Flags]
        public enum WindowStyleEx : uint
        {
            DEFAULT = 0x00000000u,
            LEFT = DEFAULT,
            LTRREADING = DEFAULT,
            RIGHTSCROLLBAR = DEFAULT,
            DLGMODALFRAME = 0x00000001u,
            NOPARENTNOTIFY = 0x00000004u,
            TOPMOST = 0x00000008u,
            ACCEPTFILES = 0x00000010u,
            TRANSPARENT = 0x00000020u,
            MDICHILD = 0x00000040u,
            TOOLWINDOW = 0x00000080u,
            WINDOWEDGE = 0x00000100u,
            CLIENTEDGE = 0x00000200u,
            CONTEXTHELP = 0x00000400u,
            RIGHT = 0x00001000u,
            RTLREADING = 0x00002000u,
            LEFTSCROLLBAR = 0x00004000u,
            LAYERED = 0x00080000u,
            CONTROLPARENT = 0x00010000u,
            STATICEDGE = 0x00020000u,
            APPWINDOW = 0x00040000u,
            NOINHERITLAYOUT = 0x00100000u,
            NOREDIRECTIONBITMAP = 0x00200000u,
            LAYOUTRTL = 0x00400000u,
            COMPOSITED = 0x02000000u,
            NOACTIVATE = 0x08000000u,
            OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
            PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST
        }

        public enum EventObject
        {
            CREATE = 0x8000,
            DESTROY = 0x8001,
            SHOW = 0x8002,
            HIDE = 0x8003,
            REORDER = 0x8004,
            FOCUS = 0x8005,
            SELECTION = 0x8006,
            SELECTIONADD = 0x8007,
            SELECTIONREMOVE = 0x8008,
            SELECTIONWITHIN = 0x8009,
            STATECHANGE = 0x800A,
            LOCATIONCHANGE = 0x800B,
            NAMECHANGE = 0x800C,
            DESCRIPTIONCHANGE = 0x800D,
            VALUECHANGE = 0x800E,
            PARENTCHANGE = 0x800F,
            HELPCHANGE = 0x8010,
            DEFACTIONCHANGE = 0x8011,
            ACCELERATORCHANGE = 0x8012,
            INVOKED = 0x8013,
            TEXTSELECTIONCHANGED = 0x8014,
            CONTENTSCROLLED = 0x8015,
            CLOAKED = 0x8017,
            UNCLOAKED = 0x8018,
            LIVEREGIONCHANGED = 0x8019,
            HOSTEDOBJECTSINVALIDATED = 0x8020,
            DRAGSTART = 0x8021,
            DRAGCANCEL = 0x8022,
            DRAGCOMPLETE = 0x8023,
            DRAGENTER = 0x8024,
            DRAGLEAVE = 0x8025,
            DRAGDROPPED = 0x8026,
            IME_SHOW = 0x8027,
            IME_HIDE = 0x8028,
            IME_CHANGE = 0x8029,
            TEXTEDIT_CONVERSIONTARGETCHANGED = 0x8030,
            END = 0x80FF
        }

        public enum WindowsMessageType
        {
            GETMINMAXINFO = 0x0024,
            LBUTTONDOWN = 0x0201,
            LBUTTONUP = 0x0202,
            RBUTTONDOWN = 0x0204,
            RBUTTONUP = 0x0205
        }

        [Flags]
        public enum WinEventFlags
        {
            OutOfContext = 0,
            SkipOwnThread = 1 << 0,
            SkipOwnProcess = 1 << 1,
            InContext = 1 << 2
        }

        [Flags]
        public enum GetAncestorFlags
        {
            Parent = 1 << 0,
            Root = 1 << 1,
            RootOwner = Parent | Root
        }

        public delegate bool SetLayeredWindowAttributesDelegate(IntPtr hWnd, uint crKey, byte bAlpha, int dwFlags);
        public delegate bool ShowWindowDelegate(IntPtr hWnd, int nCmdShow);
        public delegate IntPtr GetWindowDelegate(IntPtr hWnd, uint uCmd);
        public delegate int GetWindowLongADelegate(IntPtr hWnd, uint nIndex);
        public delegate int SetWindowLongADelegate(IntPtr hWnd, uint nIndex, int dwNewLong);
        public delegate IntPtr SetWinEventHookDelegate(uint eventMin,
                                                       uint eventMax,
                                                       IntPtr hModWinEventProc,
                                                       WinEventProcDelegate pfnWinEventProc,
                                                       uint idProcess,
                                                       uint idThread,
                                                       WinEventFlags dwFlags);
        public delegate bool UnhookWinEventDelegate(IntPtr hWinEventHook);
        public delegate IntPtr SetActiveWindowDelegate(IntPtr hWnd);
        public delegate IntPtr GetAncestorDelegate(IntPtr hWnd, GetAncestorFlags gaFlags);

        public delegate void WinEventProcDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint idEventThread, uint dwMsEventTime);

        public const uint GW_OWNER = 0x00000004u;
        public const uint GWL_STYLE = 0xFFFFFFF0u;
        public const uint GWL_EXSTYLE = 0xFFFFFFECu;
        public const uint GWL_HWNDPARENT = 0xFFFFFFF8u;
        public const uint MONITOR_DEFAULTTONEAREST = 0x00000002u;

        public static readonly SetLayeredWindowAttributesDelegate SetLayeredWindowAttributes;
        public static readonly ShowWindowDelegate ShowWindow;
        public static readonly GetWindowDelegate GetWindow;
        public static readonly GetWindowLongADelegate GetWindowLongA;
        public static readonly SetWindowLongADelegate SetWindowLongA;
        public static readonly SetWinEventHookDelegate SetWinEventHook;
        public static readonly UnhookWinEventDelegate UnhookWinEvent;
        public static readonly SetActiveWindowDelegate SetActiveWindow;
        public static readonly GetAncestorDelegate GetAncestor;
    }
}
