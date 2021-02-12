using Nanocore.Sage;
using NanoswarmHive.Presentation.Commands;
using NanoswarmHive.Presentation.Controls;
using NanoswarmHive.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SystemCommands = System.Windows.SystemCommands;

namespace NanoswarmHive
{
    public class MainWindowViewModel : ADispatcherViewModel
    {
        private ImageSource _backgroundImageSource;
        private IEnumerable<AViewModelBase> _buttons;
        private string _title;

        internal Window _window;

        private WindowButtonInfo ButtonPlay => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:Play" },
            Command = new AnonymousTaskCommand(ServiceProvider, ExecutePlay)
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
            CommandParameter = "https://www.moddb.com/mods/command-and-conquer-generals-evolution"
        };

        private WindowButtonInfo ButtonJoinDiscord => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:JoinDiscord" },
            Command = UtilityCommands.OpenHyperlinkCommand,
            CommandParameter = "https://discord.com/invite/wnmRFypsRp"
        };

        private WindowButtonInfo ButtonVisitPatreon => new WindowButtonInfo
        {
            Content = new TranslatedTextBlock { DisplayText = "GUI:VisitPatreon" },
            Command = UtilityCommands.OpenHyperlinkCommand,
            CommandParameter = "https://www.patreon.com/GunshipMarkII"
        };

        public ImageSource BackgroundImage { get => _backgroundImageSource; set => SetValue(ref _backgroundImageSource, value); }
        public IEnumerable<AViewModelBase> Buttons { get => _buttons; set => SetValue(ref _buttons, value); }
        public string Title { get => _title; set => SetValue(ref _title, value); }
        public string Readme => ServiceProvider.Get<Registry>().Readme;

        public MainWindowViewModel(IViewModelServiceProvider serviceProvider) : base(serviceProvider)
        {
            Buttons = GetButtons();
            ButtonReadme.CommandParameter = serviceProvider.Get<Registry>().Readme;
            _title = serviceProvider.Get<Registry>().DisplayName;
        }

        private ICollection<AViewModelBase> GetButtons()
        {
            return new AViewModelBase[]
            {
                ButtonPlay,
                // ButtonCheckForUpdates,
                // ButtonWorldBuilder,
                ButtonVisitWebsite,
                ButtonJoinDiscord,
                ButtonVisitPatreon,
                ButtonQuit,
                ButtonReadme,
                // ButtonGameBrowser,
                ButtonSetLanguage,
                WindowButtonFreeSpaceInfo.Default,
                WindowButtonFreeSpaceInfo.Default,
                WindowButtonFreeSpaceInfo.Default
            }.ToList();
        }

        private async Task ExecutePlay()
        {
            // SystemCommands.CloseWindow(_window);
            _window.Hide();
            await Program.Launch(ServiceProvider, true);
            _window.Show();
        }

        private void ExecuteQuit()
        {
            SystemCommands.CloseWindow(_window);
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
