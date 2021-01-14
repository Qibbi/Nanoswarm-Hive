using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace NanoswarmHive.Presentation.Controls
{
    [TemplatePart(Name = _messageContainerPartName, Type = typeof(FlowDocumentScrollViewer))]
    public class MarkdownTextBlock : Control
    {
        private const string _messageContainerPartName = "PART_MessageContainer";

        public static readonly DependencyProperty BaseUriProperty =
            DependencyProperty.Register(nameof(BaseUrl), typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(BaseUrlChanged));
        public static readonly DependencyProperty HyperlinkCommandProperty =
            DependencyProperty.Register(nameof(HyperlinkCommand), typeof(ICommand), typeof(MarkdownTextBlock), new PropertyMetadata(HyperlinkCommandChanged));
        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.Register(nameof(Markdown), typeof(XamlMarkdown), typeof(MarkdownTextBlock), new PropertyMetadata(MarkdownChanged));
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(TextChanged));

        public string BaseUrl { get => (string)GetValue(BaseUriProperty); set => SetValue(BaseUriProperty, value); }
        public ICommand HyperlinkCommand { get => (ICommand)GetValue(HyperlinkCommandProperty); set => SetValue(HyperlinkCommandProperty, value); }
        public XamlMarkdown Markdown { get => (XamlMarkdown)GetValue(MarkdownProperty); set => SetValue(MarkdownProperty, value); }
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        private readonly Lazy<XamlMarkdown> _defaultMarkdown;
        private FlowDocumentScrollViewer _messageContainer;

        static MarkdownTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(typeof(MarkdownTextBlock)));
        }

        public MarkdownTextBlock()
        {
            _defaultMarkdown = new Lazy<XamlMarkdown>(() => new XamlMarkdown(this));
        }

        private static void BaseUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MarkdownTextBlock control)
            {
                if (!(args.NewValue is null))
                {
                    control.GetMarkdown().BaseUrl = (string)args.NewValue;
                }
                control.ResetMessage();
            }
            else
            {
                throw new ArgumentNullException(nameof(d));
            }
        }

        private static void HyperlinkCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MarkdownTextBlock control)
            {
                if (!(args.NewValue is null))
                {
                    control.GetMarkdown().HyperlinkCommand = (ICommand)args.NewValue;
                }
                control.ResetMessage();
            }
            else
            {
                throw new ArgumentNullException(nameof(d));
            }
        }

        private static void MarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MarkdownTextBlock control)
            {
                if (!(args.NewValue is null))
                {
                    ((XamlMarkdown)args.NewValue).BaseUrl = control.BaseUrl;
                    ((XamlMarkdown)args.NewValue).HyperlinkCommand = control.HyperlinkCommand;
                }
                control.ResetMessage();
            }
            else
            {
                throw new ArgumentNullException(nameof(d));
            }
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MarkdownTextBlock control)
            {
                control.ResetMessage();
            }
            else
            {
                throw new ArgumentNullException(nameof(d));
            }
        }

        private XamlMarkdown GetMarkdown()
        {
            return Markdown ?? _defaultMarkdown.Value;
        }

        private void ResetMessage()
        {
            if (!(_messageContainer is null))
            {
                _messageContainer.Document = ProcessText();
            }
        }

        private FlowDocument ProcessText()
        {
            try
            {
                return GetMarkdown().Transform(Text ?? "<null>");
            }
            catch (ArgumentException)
            {
            }
            catch (FormatException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _messageContainer = GetTemplateChild(_messageContainerPartName) as FlowDocumentScrollViewer;
            if (_messageContainer is null)
            {
                throw new InvalidOperationException($"A part named {_messageContainerPartName} of type {typeof(FlowDocumentScrollViewer)} has to exist in the ControlTemplate.");
            }
            ResetMessage();
        }
    }
}
