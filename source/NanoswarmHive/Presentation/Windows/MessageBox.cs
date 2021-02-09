using Nanocore.Core.Extensions;
using NanoswarmHive.Presentation.Controls;
using NanoswarmHive.Presentation.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NanoswarmHive.Presentation.Windows
{
    public class MessageBox : AMessageDialogBase
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(MessageBox));

        public ImageSource Image { get => (ImageSource)GetValue(ImageProperty); set => SetValue(ImageProperty, value); }

        public static DialogButtonInfo ButtonOk => new DialogButtonInfo
        {
            IsDefault = true,
            Result = Services.DialogResultType.Ok,
            Content = new TranslatedTextBlock { DisplayText = "GUI:Ok" }
        };
        public static DialogButtonInfo ButtonCancel => new DialogButtonInfo
        {
            IsCancel = true,
            Result = Services.DialogResultType.Cancel,
            Content = new TranslatedTextBlock { DisplayText = "GUI:Cancel" }
        };
        public static DialogButtonInfo ButtonYes => new DialogButtonInfo
        {
            IsDefault = true,
            Result = Services.DialogResultType.Yes,
            Content = new TranslatedTextBlock { DisplayText = "GUI:Yes" },
            Key = "KEY:Y".Translate()
        };
        public static DialogButtonInfo ButtonNo => new DialogButtonInfo
        {
            Result = Services.DialogResultType.No,
            Content = new TranslatedTextBlock { DisplayText = "GUI:No" },
            Key = "KEY:N".Translate()
        };

        protected MessageBox() : base()
        {
        }

        internal static ICollection<DialogButtonInfo> GetButtons(Services.MessageBoxButtonType button)
        {
            ICollection<DialogButtonInfo> result;
            switch (button)
            {
                case Services.MessageBoxButtonType.Ok:
                    DialogButtonInfo ok = ButtonOk;
                    ok.IsCancel = true;
                    result = new[] { ok };
                    break;
                case Services.MessageBoxButtonType.OkCancel:
                    result = new[] { ButtonOk, ButtonCancel };
                    break;
                case Services.MessageBoxButtonType.YesNo:
                    DialogButtonInfo no = ButtonNo;
                    no.IsCancel = true;
                    result = new[] { ButtonYes, no };
                    break;
                case Services.MessageBoxButtonType.YesNoCancel:
                    result = new[] { ButtonYes, ButtonNo, ButtonCancel };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
            return result;
        }

        internal static void SetImage(MessageBox messageBox, Services.MessageBoxImageType image)
        {
            string imageKey;
            switch (image)
            {
                case Services.MessageBoxImageType.None:
                    imageKey = null;
                    break;
                case Services.MessageBoxImageType.Error:
                    imageKey = "ImageErrorDialog";
                    break;
                case Services.MessageBoxImageType.Warning:
                    imageKey = "ImageWarningDialog";
                    break;
                case Services.MessageBoxImageType.Question:
                    imageKey = "ImageQuestionDialog";
                    break;
                case Services.MessageBoxImageType.Information:
                    imageKey = "ImageInformationDialog";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(image), image, null);
            }
            messageBox.Image = imageKey is null ? null : (ImageSource)messageBox.TryFindResource(imageKey);
        }

        internal static void SetKeyBindings(MessageBox messageBox, IEnumerable<DialogButtonInfo> buttons)
        {
            foreach (DialogButtonInfo button in buttons)
            {
                if (!Enum.TryParse(button.Key, out Key key))
                {
                    continue;
                }
                KeyBinding binding = new KeyBinding(messageBox.ButtonCommand, key, ModifierKeys.Alt)
                {
                    CommandParameter = button.Result
                };
                messageBox.InputBindings.Add(binding);
            }
        }

        public static async Task<Services.MessageBoxResultType> Show(string message,
                                                                     string caption,
                                                                     Services.MessageBoxButtonType buttons,
                                                                     Services.MessageBoxImageType image,
                                                                     string details = null)
        {
            return await Show(message, caption, GetButtons(buttons), image, details);
        }

        public static async Task<Services.MessageBoxResultType> Show(string message,
                                                                     string caption,
                                                                     IEnumerable<DialogButtonInfo> buttons,
                                                                     Services.MessageBoxImageType image,
                                                                     string details = null)
        {
            List<DialogButtonInfo> list = buttons.ToList();
            MessageBox messageBox = new MessageBox
            {
                Title = caption,
                Content = message,
                ButtonsSource = list,
                Details = details
            };
            SetImage(messageBox, image);
            SetKeyBindings(messageBox, list);
            await messageBox.ShowModal();
            return (Services.MessageBoxResultType)messageBox.Result;
        }

        private void Copy(object sender, ExecutedRoutedEventArgs args)
        {
            object data = Content;
            if (!string.IsNullOrWhiteSpace(Details))
            {
                if (data is string str)
                {
                    data = $"{str}\n\r\n\r{Details}";
                }
            }
            SafeClipboard.SetDataObject(data ?? string.Empty, true);
        }

        protected override void OnInitialized(EventArgs args)
        {
            base.OnInitialized(args);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy));
        }

        protected override void OnClosed(EventArgs args)
        {
            base.OnClosed(args);
        }
    }
}
