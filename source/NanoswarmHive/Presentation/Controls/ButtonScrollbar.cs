using NanoswarmHive.Presentation.Commands;
using NanoswarmHive.Presentation.View;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NanoswarmHive.Presentation.Controls
{
    [TemplatePart(Name = _scrollViewerPartName, Type = typeof(ScrollViewer))]
    public class ButtonScrollbar : ItemsControl
    {
        private const string _scrollViewerPartName = "PART_ScrollViewer";

        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register(nameof(ItemsPerPage), typeof(int), typeof(ButtonScrollbar), new PropertyMetadata(5));

        private readonly ICommandBase _scrollLeft;
        private readonly ICommandBase _scrollRight;

        private readonly IViewModelServiceProvider _serviceProvider;
        private ScrollViewer _scrollViewer;
        private int _scrollSteps;
        private int _currentScrollStep;
        private double _scrollRange;

        public WindowButtonInfo ButtonLeft => new WindowButtonInfo
        {
            Command = _scrollLeft
        };

        public WindowButtonInfo ButtonRight => new WindowButtonInfo
        {
            Command = _scrollRight
        };
        public int ItemsPerPage { get => (int)GetValue(ItemsPerPageProperty); set => SetValue(ItemsPerPageProperty, value); }

        public event RoutedEventHandler ScrollStarted;
        public event RoutedEventHandler ScrollEnded;

        public ButtonScrollbar()
        {
            _serviceProvider = new ViewModelServiceProvider(new[] { new DispatcherService(Dispatcher) });
            _scrollLeft = new AnonymousTaskCommand(_serviceProvider, ScrollLeftAsync, CanScrollLeft);
            _scrollRight = new AnonymousTaskCommand(_serviceProvider, ScrollRightAsync, CanScrollRight);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // _scrollSteps = (int)(_scrollViewer.ExtentWidth / _scrollViewer.ActualWidth) - 1;
            _scrollSteps = (Items.Count / ItemsPerPage) - ((Items.Count % ItemsPerPage == 0) ? 1 : 0);
            _currentScrollStep = 0;
            _scrollRange = _scrollViewer.ActualWidth;
            _scrollLeft.IsEnabled = false;
            _scrollLeft.IsEnabled = true;
            _scrollRight.IsEnabled = false;
            _scrollRight.IsEnabled = true;
        }

        private async Task ScrollLeftAsync()
        {
            _scrollLeft.IsEnabled = false;
            _scrollRight.IsEnabled = false;
            const int scrollTime = 1000;
            double scrollLength = _scrollRange / scrollTime * 25;
            double currentPosition = 2.0 + (_currentScrollStep * _scrollViewer.ActualWidth) - scrollLength;
            for (int idx = 0; idx < 40; ++idx)
            {
                _scrollViewer.ScrollToHorizontalOffset(currentPosition);
                currentPosition -= scrollLength;
                await Task.Delay(15);
            }
            --_currentScrollStep;
            _scrollLeft.IsEnabled = true;
            _scrollRight.IsEnabled = true;
        }

        private bool CanScrollLeft()
        {
            return _currentScrollStep > 0;
        }

        private async Task ScrollRightAsync()
        {
            _scrollLeft.IsEnabled = false;
            _scrollRight.IsEnabled = false;
            const int scrollTime = 1000;
            double scrollLength = _scrollRange / scrollTime * 25;
            double currentPosition = 2.0 + (_currentScrollStep * _scrollViewer.ActualWidth) + scrollLength;
            for (int idx = 0; idx < 40; ++idx)
            {
                _scrollViewer.ScrollToHorizontalOffset(currentPosition);
                currentPosition += scrollLength;
                await Task.Delay(15);
            }
            ++_currentScrollStep;
            _scrollLeft.IsEnabled = true;
            _scrollRight.IsEnabled = true;
        }

        private bool CanScrollRight()
        {
            return _currentScrollStep < _scrollSteps;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = GetTemplateChild(_scrollViewerPartName) as ScrollViewer;
            if (_scrollViewer is null)
            {
                throw new InvalidOperationException($"A part named {_scrollViewerPartName} of type {typeof(ScrollViewer)} has to exist in the ControlTemplate.");
            }
            _scrollViewer.ScrollToHorizontalOffset(2.0);
        }
    }
}
