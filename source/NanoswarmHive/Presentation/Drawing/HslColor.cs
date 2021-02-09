using Nanocore.Core.Mathematics;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.Drawing
{
    public struct HslColor
    {
        private double _hue;
        private double _saturation;
        private double _luminosity;
        private double _alpha;

        public double Hue
        {
            get => _hue;
            set => _hue = MathUtil.Clamp(value, 0.0, 1.0);
        }
        public double Saturation
        {
            get => _saturation;
            set => _saturation = MathUtil.Clamp(value, 0.0, 1.0);
        }
        public double Luminosity
        {
            get => _luminosity;
            set => _luminosity = MathUtil.Clamp(value, 0.0, 1.0);
        }
        public double Alpha
        {
            get => _alpha;
            set => _alpha = MathUtil.Clamp(value, 0.0, 1.0);
        }

        public HslColor(double hue, double saturation, double luminosity, double alpha)
        {
            _hue = MathUtil.Clamp(hue, 0.0, 360.0);
            _saturation = MathUtil.Clamp(saturation, 0.0, 1.0);
            _luminosity = MathUtil.Clamp(luminosity, 0.0, 1.0);
            _alpha = MathUtil.Clamp(alpha, 0.0, 1.0);
        }

        private static double ModOne(double value)
        {
            return value < 0.0 ? value + 1.0 : value > 1.0 ? value - 1.0 : value;
        }

        private static double ComputeRGBComponent(double p, double q, double tC)
        {
            if (tC < 1.0 / 6.0)
            {
                return p + ((q - p) * 6.0 * tC);
            }
            if (tC < 0.5)
            {
                return q;
            }
            if (tC < 2.0 / 3.0)
            {
                return p + ((q - p) * 6.0 * ((2.0 / 3.0) - tC));
            }
            return p;
        }

        public static HslColor FromColor(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte max = MathUtil.Max3(r, g, b);
            byte min = MathUtil.Min3(r, g, b);
            int range = max - min;
            double maxD = max / (double)byte.MaxValue;
            double minD = min / (double)byte.MaxValue;
            double hue = max != min ? max != r ? max != g ? (60.0 * (r - g) / range) + 240.0 : (60.0 * (b - r) / range) + 120.0 : MathUtil.FMod((float)((60.0 * (g - b) / range) + 360.0), 360.0f) : 0.0;
            double luminosity = 0.5 * (maxD + minD);
            double saturation = max != min ? (luminosity > 0.5 ? (maxD - minD) / (2.0 - (2.0 * luminosity)) : (maxD - minD) / (2.0 * luminosity)) : 0.0;
            double alpha = color.A / (double)byte.MaxValue;
            return new HslColor(hue, saturation, luminosity, alpha);
        }

        public Color ToColor()
        {
            double q = _luminosity < 0.5 ? _luminosity * (1.0 + _saturation) : _luminosity + _saturation - (_luminosity * _saturation);
            double p = (2.0 * _luminosity) - q;
            double num = _hue / 360.0;
            return Color.FromArgb((byte)(Alpha * byte.MaxValue),
                                  (byte)(ComputeRGBComponent(p, q, ModOne(num + (1.0 / 3.0))) * byte.MaxValue),
                                  (byte)(ComputeRGBComponent(p, q, num) * byte.MaxValue),
                                  (byte)(ComputeRGBComponent(p, q, ModOne(num - (1.0 / 3.0))) * byte.MaxValue));
        }
    }
    public static class HslExtensions
    {
        public static HslColor ToHslColor(this Color color)
        {
            return HslColor.FromColor(color);
        }
    }
}
