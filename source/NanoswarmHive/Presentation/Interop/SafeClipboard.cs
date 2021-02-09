using Nanocore.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace NanoswarmHive.Presentation.Interop
{
    public static class SafeClipboard
    {
        public const int CLIPBRD_E_CANT_OPEN = unchecked((int)0x800401D0);
        public const int CLIPBRD_E_CANT_SET = unchecked((int)0x800401D2);

        public static bool ContainsText()
        {
            try
            {
                return Clipboard.ContainsText();
            }
            catch (COMException e) when (e.HResult == CLIPBRD_E_CANT_OPEN)
            {
                return false;
            }
        }

        public static string GetText()
        {
            try
            {
                return Clipboard.GetText();
            }
            catch (COMException e) when (e.HResult == CLIPBRD_E_CANT_OPEN)
            {
                e.Ignore();
                return string.Empty;
            }
        }

        public static void SetText(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            try
            {
                Clipboard.SetText(text);
            }
            catch (COMException e) when (e.HResult == CLIPBRD_E_CANT_OPEN || e.HResult == CLIPBRD_E_CANT_SET)
            {
                e.Ignore();
            }
        }

        public static void SetDataObject(object data, bool isCopy)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            try
            {
                Clipboard.SetDataObject(data, isCopy);
            }
            catch (COMException e) when (e.HResult == CLIPBRD_E_CANT_OPEN || e.HResult == CLIPBRD_E_CANT_SET)
            {
                e.Ignore();
            }
        }
    }
}
