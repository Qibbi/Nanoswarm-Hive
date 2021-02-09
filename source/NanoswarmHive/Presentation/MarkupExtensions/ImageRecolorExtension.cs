using NanoswarmHive.Presentation.Themes;
using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(Image))]
    public class ImageRecolorExtension : MarkupExtension
    {
        private readonly ImageSource _source;
        private readonly int _width;
        private readonly int _height;
        private readonly BitmapScalingMode _scalingMode;

        public ImageRecolorExtension(ImageSource source)
        {
            _source = source;
            _width = -1;
            _height = -1;
        }

        public ImageRecolorExtension(ImageSource source, int width, int height) : this(source, width, height, BitmapScalingMode.Unspecified)
        {
        }

        public ImageRecolorExtension(ImageSource source, int width, int height, BitmapScalingMode scalingMode)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            _source = source;
            _width = width;
            _height = height;
            _scalingMode = scalingMode;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Image image = new Image { Source = _source };
            if (_source is DrawingImage drawingImage)
            {
                image.Source = new DrawingImage() { Drawing = ImageThemingUtilities.RecolorDrawing(drawingImage.Drawing, IconThemeSelector.CurrentTheme) };
            }
            RenderOptions.SetBitmapScalingMode(image, _scalingMode);
            if (_width >= 0 && _height >= 0)
            {
                image.Width = _width;
                image.Height = _height;
            }
            return image;
        }
    }
}
