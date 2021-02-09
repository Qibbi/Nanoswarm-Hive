using NanoswarmHive.Presentation.Drawing;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.Themes
{
    public struct IconTheme
    {
        public string Name { get; }
        public Color ForegroundColor { get; }
        public Color BackgroundColor { get; }
        public double BackgroundLuminosity => BackgroundColor.ToHslColor().Luminosity;

        public IconTheme(string name, Color foregroundColor, Color backgroundColor)
        {
            Name = name;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}
