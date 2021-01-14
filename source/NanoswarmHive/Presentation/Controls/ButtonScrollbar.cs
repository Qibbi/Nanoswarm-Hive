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
    [TemplatePart(Name = _scrollLeftButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = _scrollRightButtonPartName, Type = typeof(Button))]
    public class ButtonScrollbar : ItemsControl
    {
        private const string _scrollViewerPartName = "PART_ScrollViewer";
        private const string _scrollLeftButtonPartName = "PART_ScrollLeft";
        private const string _scrollRightButtonPartName = "PART_ScrollRight";

        private readonly ICommandBase _scrollLeft;
        private readonly ICommandBase _scrollRight;

        private readonly IViewModelServiceProvider _serviceProvider;
        private ScrollViewer _scrollViewer;
        private Button _scrollLeftButton;
        private Button _scrollRightButton;
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
            _scrollSteps = (int)(_scrollViewer.ExtentWidth / _scrollViewer.ActualWidth) - 1;
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
            _scrollLeftButton = GetTemplateChild(_scrollLeftButtonPartName) as Button;
            if (_scrollLeftButton is null)
            {
                throw new InvalidOperationException($"A part named {_scrollLeftButtonPartName} of type {typeof(Button)} has to exist in the ControlTemplate.");
            }
            _scrollLeftButton.DataContext = ButtonLeft;
            _scrollRightButton = GetTemplateChild(_scrollRightButtonPartName) as Button;
            if (_scrollRightButton is null)
            {
                throw new InvalidOperationException($"A part named {_scrollRightButtonPartName} of type {typeof(Button)} has to exist in the ControlTemplate.");
            }
            _scrollRightButton.DataContext = ButtonRight;
        }
    }
}
