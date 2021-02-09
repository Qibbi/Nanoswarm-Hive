using System.Collections.Generic;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.Themes
{
    public static class IconThemeSelector
    {
        private static readonly Dictionary<string, IconTheme> _cachedThemes = new Dictionary<string, IconTheme>();

        public static IconTheme CurrentTheme { get; set; }

        static IconThemeSelector()
        {
            CurrentTheme = GetIconTheme("EVA");
        }

        // TODO: be able to set defaults
        public static IconTheme GetIconTheme(this string theme)
        {
            if (_cachedThemes.TryGetValue(theme, out IconTheme result))
            {
                return result;
            }
            result = new IconTheme(theme, Color.FromRgb(0x00, 0xC0, 0x0F), Color.FromRgb(0x00, 0x00, 0x00));
            _cachedThemes[theme] = result;
            return result;
        }
    }
}
