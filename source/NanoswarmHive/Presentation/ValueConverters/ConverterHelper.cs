using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public static class ConverterHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ConvertToBoolean(object value, IFormatProvider culture)
        {
            return value != DependencyProperty.UnsetValue && Convert.ToBoolean(value, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvertToInt32(object value, IFormatProvider culture)
        {
            return (value == DependencyProperty.UnsetValue) ? 0 : Convert.ToInt32(value, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ConvertToInt64(object value, IFormatProvider culture)
        {
            return (value == DependencyProperty.UnsetValue) ? 0L : Convert.ToInt64(value, culture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToString(object value, IFormatProvider culture)
        {
            return (value == DependencyProperty.UnsetValue) ? string.Empty : Convert.ToString(value, culture);
        }
    }
}
