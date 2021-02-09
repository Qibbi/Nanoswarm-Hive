using System.Windows;

namespace NanoswarmHive.Presentation.Themes
{
    public static class ThemeController
    {
        public static readonly DependencyProperty IsDarkProperty =
            DependencyProperty.RegisterAttached("IsDark", typeof(bool), typeof(ThemeController), new PropertyMetadata(false));

        public static bool GetIsDark(DependencyObject target)
        {
            return (bool)target.GetValue(IsDarkProperty);
        }

        public static void SetIsDark(DependencyObject target, bool value)
        {
            target.SetValue(IsDarkProperty, value);
        }
    }
}
