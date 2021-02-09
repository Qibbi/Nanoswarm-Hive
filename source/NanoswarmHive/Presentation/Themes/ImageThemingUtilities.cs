using Nanocore.Core.Mathematics;
using NanoswarmHive.Presentation.Drawing;
using System.Windows.Media;
using MColor = System.Windows.Media.Color;
using MDrawing = System.Windows.Media.Drawing;

namespace NanoswarmHive.Presentation.Themes
{
    public static class ImageThemingUtilities
    {
        private static bool IsDark(double luminosity)
        {
            return luminosity < 0.5;
        }

        private static double TransformLuminosity(HslColor hsl, double backgroundLuminosity)
        {
            double hue = hsl.Hue;
            double saturation = hsl.Saturation;
            double luminosity = hsl.Luminosity;
            double eightyTwoOverEightyFive = 82.0 / 85.0;
            if (backgroundLuminosity < 0.5)
            {
                if (luminosity >= eightyTwoOverEightyFive)
                {
                    return backgroundLuminosity * (luminosity - 1.0) / (-3.0 / 85.0);
                }
                double saturationRange = saturation >= 0.2 ? saturation <= 0.3 ? 1.0 - ((saturation - 0.2) / 0.1) : 0.0 : 1.0;
                double num1 = MathUtil.Max(MathUtil.Min(1.0, MathUtil.Abs(hue - 37.0) / 20.0), saturationRange);
                double num2 = ((((backgroundLuminosity - 1.0) * 0.66 / eightyTwoOverEightyFive) + 1.0) * num1) + (0.66 * (1.0 - num1));
                if (luminosity < 0.66)
                {
                    return ((num2 - 1.0) / 0.66 * luminosity) + 1.0;
                }
                return ((num2 - backgroundLuminosity) / (-259.0 / 850.0) * (luminosity - eightyTwoOverEightyFive)) + backgroundLuminosity;
            }
            if (luminosity < eightyTwoOverEightyFive)
            {
                return luminosity * backgroundLuminosity / eightyTwoOverEightyFive;
            }
            return ((1.0 - backgroundLuminosity) * (luminosity - 1.0) / (3.0 / 85.0)) + 1.0;
        }

        private static void TransformParts(MDrawing drawing, IconTheme theme)
        {
            if (drawing is GeometryDrawing gd && gd.Brush is SolidColorBrush s)
            {
                HslColor hsl = s.Color.ToHslColor();
                double transformedLuminosity = TransformLuminosity(hsl, theme.BackgroundLuminosity);
                s.Color = new HslColor(hsl.Hue, hsl.Saturation, transformedLuminosity, hsl.Alpha).ToColor();
            }
            else if (drawing is DrawingGroup dg)
            {
                foreach (MDrawing dr in dg.Children)
                {
                    if (dr is DrawingGroup || dr is GeometryDrawing)
                    {
                        TransformParts(dr, theme);
                    }
                }
            }
        }

        private static void RecolorParts(MDrawing drawing, IconTheme theme)
        {
            MColor color = theme.ForegroundColor;
            if (drawing is GeometryDrawing gd)
            {
                if (gd.Brush is SolidColorBrush s)
                {
                    MColor recolor = s.Color;
                    recolor.R = (byte)(recolor.R * (color.R / 255.0));
                    recolor.G = (byte)(recolor.G * (color.G / 255.0));
                    recolor.B = (byte)(recolor.B * (color.B / 255.0));
                    recolor.A = (byte)(recolor.A * (color.A / 255.0));
                    s.Color = recolor;
                }
                else if (gd.Brush is LinearGradientBrush l)
                {
                    foreach (GradientStop gs in l.GradientStops)
                    {
                        MColor recolor = gs.Color;
                        recolor.R = (byte)(recolor.R * (color.R / 255.0));
                        recolor.G = (byte)(recolor.G * (color.G / 255.0));
                        recolor.B = (byte)(recolor.B * (color.B / 255.0));
                        recolor.A = (byte)(recolor.A * (color.A / 255.0));
                        gs.Color = recolor;
                    }
                }
            }
            else if (drawing is DrawingGroup dg)
            {
                foreach (MDrawing dr in dg.Children)
                {
                    if (dr is DrawingGroup || dr is GeometryDrawing)
                    {
                        RecolorParts(dr, theme);
                    }
                }
            }
        }

        public static MDrawing TransformDrawing(MDrawing drawing, IconTheme theme, bool isCheckingLuminosity = true)
        {
            bool isDark = ThemeController.GetIsDark(drawing);
            if (isCheckingLuminosity)
            {
                if (isDark == IsDark(theme.BackgroundLuminosity))
                {
                    return drawing;
                }
            }
            MDrawing result = drawing.CloneCurrentValue();
            TransformParts(result, theme);
            ThemeController.SetIsDark(result, !isDark);
            return result;
        }

        public static MDrawing RecolorDrawing(MDrawing drawing, IconTheme theme, bool isCheckingLuminosity = true)
        {
            MDrawing result = TransformDrawing(drawing, theme, isCheckingLuminosity);
            RecolorParts(result, theme);
            return result;
        }
    }
}
