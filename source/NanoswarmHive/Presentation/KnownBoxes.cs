using System;
using System.Windows;

namespace NanoswarmHive.Presentation
{
    internal static class BooleanBoxes
    {
        internal static readonly object _trueBox = true;
        internal static readonly object _falseBox = false;

        internal static object Box(this bool value)
        {
            return value ? _trueBox : _falseBox;
        }
    }

    internal static class VisibilityBoxes
    {
        internal static object _visibleBox = Visibility.Visible;
        internal static object _hiddenBox = Visibility.Hidden;
        internal static object _collapsedBox = Visibility.Collapsed;

        internal static object Box(this Visibility value)
        {
            switch (value)
            {
                case Visibility.Visible:
                    return _visibleBox;
                case Visibility.Hidden:
                    return _hiddenBox;
                case Visibility.Collapsed:
                    return _collapsedBox;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
