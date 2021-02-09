using Nanocore.Sage;
using NanoswarmHive.Presentation.Commands;
using NanoswarmHive.Presentation.Controls;
using NanoswarmHive.Presentation.Services;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SystemCommands = System.Windows.SystemCommands;

namespace NanoswarmHive
{
    public class MainWindowViewModel : ADispatcherViewModel
    {
        private DialogResultType _dialogResult;
        private ImageSource _backgroundImageSource;
        private IEnumerable<AViewModelBase> _buttons;

        internal Window _window;

        private WindowButtonInfo ButtonPlay => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:Play" },
            Command = new AnonymousCommand(ServiceProvider, ExecutePlay)
        };

        private WindowButtonInfo ButtonCheckForUpdates => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:CheckForUpdates" },
            ToolTip = new TranslatedTextBlock { DisplayText = "GUI:NotImplemented" },
            Command = UtilityCommands.DisabledCommand
        };

        private WindowButtonInfo ButtonSetLanguage => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:SetLanguage" },
            ToolTip = new TranslatedTextBlock { DisplayText = "GUI:NotImplemented" },
            Command = UtilityCommands.DisabledCommand
        };

        private WindowButtonInfo ButtonWorldBuilder => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:WorldBuilder" },
            ToolTip = new TranslatedTextBlock { DisplayText = "GUI:NotImplemented" },
            Command = UtilityCommands.DisabledCommand
        };

        private WindowButtonInfo ButtonQuit => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:Quit" },
            Command = new AnonymousCommand(ServiceProvider, ExecuteQuit)
        };

        private WindowButtonInfo ButtonGameBrowser => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:GameBrowser" },
            ToolTip = new TranslatedTextBlock { DisplayText = "GUI:NotImplemented" },
            Command = UtilityCommands.DisabledCommand
        };

        private WindowButtonInfo ButtonReadme => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:Readme" },
            Command = UtilityCommands.OpenHyperlinkCommand,
            CommandParameter = ServiceProvider.Get<Registry>().Readme
        };

        private WindowButtonInfo ButtonVisitWebsite => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:VisitWebsite" },
            Command = UtilityCommands.OpenHyperlinkCommand,
            CommandParameter = ""
        };

        private WindowButtonInfo ButtonJoinDiscord => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:JoinDiscord" },
            Command = UtilityCommands.OpenHyperlinkCommand,
            CommandParameter = "https://discord.gg/cUMrGFcqU3"
        };

        public DialogResultType DialogResult => _dialogResult;
        public ImageSource BackgroundImage { get => _backgroundImageSource; set => SetValue(ref _backgroundImageSource, value); }
        public IEnumerable<AViewModelBase> Buttons { get => _buttons; set => SetValue(ref _buttons, value); }

        public MainWindowViewModel(IViewModelServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dialogResult = DialogResultType.None;
            ButtonReadme.CommandParameter = serviceProvider.Get<Registry>().Readme;
            Buttons = GetButtons();
        }

        private ICollection<AViewModelBase> GetButtons()
        {
            return new AViewModelBase[]
            {
                ButtonPlay,
                ButtonCheckForUpdates,
                ButtonSetLanguage,
                ButtonWorldBuilder,
                ButtonQuit,
                ButtonGameBrowser,
                ButtonReadme,
                ButtonVisitWebsite,
                ButtonJoinDiscord,
                WindowButtonFreeSpaceInfo.Default,
                WindowButtonFreeSpaceInfo.Default
            }.ToList();
        }

        private void ExecutePlay()
        {
            _dialogResult = DialogResultType.Yes;
            SystemCommands.CloseWindow(_window);
        }

        private void ExecuteQuit()
        {
            SystemCommands.CloseWindow(_window);
        }

        public bool IsStartGame()
        {
            return _dialogResult == DialogResultType.Yes;
        }

        public void SetStartGame()
        {
            _dialogResult = DialogResultType.Yes;
        }

        public void LoadBackground()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Launcher");
            string imagePath = Path.Combine(path, $"{ServiceProvider.Get<Registry>().Language}_cnc.bmp");
            if (!File.Exists(imagePath))
            {
                imagePath = Path.Combine(path, "cnc.bmp");
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
                BackgroundImage = image;
            }
        }
    }
}
