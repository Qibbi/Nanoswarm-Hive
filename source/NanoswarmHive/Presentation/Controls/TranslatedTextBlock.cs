using Nanocore.Core.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace NanoswarmHive.Presentation.Controls
{
    public class TranslatedTextBlock : TextBlock
    {
        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(nameof(DisplayText),
                                        typeof(string),
                                        typeof(TranslatedTextBlock),
                                        new FrameworkPropertyMetadata(string.Empty,
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                                      new PropertyChangedCallback(OnDisplayTextChanged)));
        public static readonly DependencyProperty IsUpperCaseProperty =
            DependencyProperty.Register(nameof(IsUpperCase),
                                        typeof(bool),
                                        typeof(TranslatedTextBlock),
                                        new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisplayTextChanged)));

        public string DisplayText { get => (string)GetValue(DisplayTextProperty); set => SetValue(DisplayTextProperty, value); }
        public bool IsUpperCase { get => (bool)GetValue(IsUpperCaseProperty); set => SetValue(IsUpperCaseProperty, value); }

        public TranslatedTextBlock() : base()
        {
            Loaded += OnLoaded;
        }

        public TranslatedTextBlock(System.Windows.Documents.Inline inline) : base(inline)
        {
            Loaded += OnLoaded;
        }

        private static void OnDisplayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as TranslatedTextBlock)?.OnDisplayTextChanged(args);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnDisplayTextChanged(this, new DependencyPropertyChangedEventArgs(DisplayTextProperty, null, DisplayText));
            Loaded -= OnLoaded;
        }

        protected virtual void OnDisplayTextChanged(DependencyPropertyChangedEventArgs args)
        {
            string text = (args.NewValue as string).Translate();
            if (IsUpperCase)
            {
                text = text.ToUpper();
            }
            Text = text;
        }
    }
}
