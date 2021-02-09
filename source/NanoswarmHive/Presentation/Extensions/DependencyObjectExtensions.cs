using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.Extensions
{
    public static class DependencyObjectExtensions
    {
        private static IEnumerable<T> FindChildrenOfType<T>(DependencyObject source,
                                                            Func<DependencyObject, int> getChildrenCount,
                                                            Func<DependencyObject, int, DependencyObject> getChild) where T : DependencyObject
        {
            int count = getChildrenCount(source);
            for (int idx = 0; idx < count; ++idx)
            {
                DependencyObject child = getChild(source, idx);
                if (child is null)
                {
                    continue;
                }
                if (child is T)
                {
                    yield return child as T;
                }
                foreach (T subChild in FindChildrenOfType<T>(child, getChildrenCount, getChild))
                {
                    yield return subChild;
                }
            }
        }

        private static T FindParentOfType<T>(DependencyObject source, Func<DependencyObject, DependencyObject> getParent) where T : DependencyObject
        {
            while (true)
            {
                DependencyObject parent = getParent(source);
                if (parent is null)
                {
                    return null;
                }
                if (parent is T)
                {
                    return parent as T;
                }
                source = parent;
            }
        }

        public static IEnumerable<T> FindVisualChildrenOfType<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return FindChildrenOfType<T>(source, VisualTreeHelper.GetChildrenCount, VisualTreeHelper.GetChild);
        }

        public static T FindVisualParentOfType<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return FindParentOfType<T>(source, VisualTreeHelper.GetParent);
        }
    }
}
