using System;
using System.Windows;

namespace NanoswarmHive
{
    public partial class SplashScreen : Window
    {
        public SplashScreen(SplashScreenViewModel splashViewModel)
        {
            if (splashViewModel is null)
            {
                throw new ArgumentNullException(nameof(splashViewModel));
            }
            DataContext = splashViewModel;
            InitializeComponent();
            Width = Math.Min(Width, SystemParameters.WorkArea.Width);
            Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        }
    }
}
