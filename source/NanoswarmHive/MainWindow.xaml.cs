using System;
using System.Windows;

namespace NanoswarmHive
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel MainViewModel => (MainWindowViewModel)DataContext;

        public MainWindow(MainWindowViewModel mainViewModel)
        {
            if (mainViewModel is null)
            {
                throw new ArgumentNullException(nameof(MainViewModel));
            }
            DataContext = mainViewModel;
            mainViewModel._window = this;
            InitializeComponent();
            Width = Math.Min(Width, SystemParameters.WorkArea.Width);
            Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        }
    }
}
