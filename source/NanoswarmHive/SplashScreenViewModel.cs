using Nanocore.Sage;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NanoswarmHive
{
    public class SplashScreenViewModel : ADispatcherViewModel
    {
        private ImageSource _imageSource;
        private double _width;
        private double _height;
        private string _title;

        public ImageSource Image { get => _imageSource; set => SetValue(ref _imageSource, value); }
        public double Width { get => _width; set => SetValue(ref _width, value); }
        public double Height { get => _height; set => SetValue(ref _height, value); }
        public string Title { get => _title; set => SetValue(ref _title, value); }

        public SplashScreenViewModel(IViewModelServiceProvider serviceProvider) : base(serviceProvider)
        {
            _title = serviceProvider.Get<Registry>().DisplayName;
        }

        public void LoadSplash()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Launcher");
            string imagePath = Path.Combine(path, $"{ServiceProvider.Get<Registry>().Language}_splash.bmp");
            if (!File.Exists(imagePath))
            {
                imagePath = Path.Combine(path, "splash.bmp");
                if (!File.Exists(imagePath))
                {
                    return;
                }
            }
            using (Stream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                Image = image;
            }
            Width = Image.Width;
            Height = Image.Height;
        }
    }
}
