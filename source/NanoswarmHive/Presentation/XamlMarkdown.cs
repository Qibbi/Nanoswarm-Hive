using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NanoswarmHive.Presentation
{
    public sealed class XamlMarkdown : DependencyObject
    {
        private const int _maxNestDepth = 6;
        private const int _tabWidth = 4;
        private const string _markerUnorderedList = @"[*+-]";
        private const string _markerOrderedList = @"\d+[.]";

        private static readonly string _listWhole = string.Format(@"(([ ]{{0,{1}}}({0})[ ]+)(?s:.+?)(\z|\n{{2,}}(?=\S)(?![ ]*{0}[ ]+)))",
                                                                  $"(?:{_markerUnorderedList}|{_markerOrderedList})",
                                                                  _tabWidth - 1);
        private static readonly Regex _newLinesLeadingTrailing = new Regex(@"^\n+|\n+\z", RegexOptions.Compiled);
        private static readonly Regex _newLinesMultiple = new Regex(@"\n{2,}", RegexOptions.Compiled);
        private static readonly Regex _leadingWhitespace = new Regex(@"^[ ]*", RegexOptions.Compiled);
        private static readonly Regex _eoln = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _hardBreak = new Regex(@" {2,}\n", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _bold = new Regex(@"(\*\*|__)(?=\S)(.+?[*_]*)(?<=\S)\1", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _strictBold = new Regex(@"([\W_]|^)(\*\*|__)(?=\S)([^\r]*?\S[\*_]*)\2([\W_]|$)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _italic = new Regex(@"(\*|_)(?=\S)(.+?)(?<=\S)\1", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _strictItalic = new Regex(@"([\W_]|^)(\*|_)(?=\S)([^\r\*_]*?\S)\2([\W_]|$)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _anchorInline = new Regex(string.Format(@"(\[({0})\]\([ ]*({1})[ ]*((['""])(.*?)\5[ ]*)?\))",
                                                                GetNestedBracketsPattern(),
                                                                GetNestedParensPattern()), RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _codeSpan = new Regex(@"(?<!\\)(`+)(.+?)(?<!`)\1(?!`)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _imageInline = new Regex(string.Format(@"(!\[(.*?)\]\s?\([ ]*({0})[ ]*((['""])(.*?)\5[ ]*)?\))",
                                                               GetNestedParensPattern()), RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _htmlImageInline = new Regex(@"(<img[^>]*?src\s*=\s*(['""])([^'"" >]+?)\2[^>]*?>)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _listNested = new Regex(@"^" + _listWhole, RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _listTopLevel = new Regex(@"(?:(?<=\n\n)|\A\n?)" + _listWhole, RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _horizontalRules = new Regex(@"^[ ]{0,3}([-*_])(?>[ ]{0,2}\1){2,}[ ]*$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _headerAtx = new Regex(@"^(\#{1,6})[ ]*(.+?)[ ]*\#*\n+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _headerSetext = new Regex(@"^(.+?)[ ]*\n(=+|-+)[ ]*\n+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static string _nestedBracketsPattern;
        private static string _nestedParensPattern;

        public static readonly DependencyProperty BaseUrlProperty =
            DependencyProperty.Register(nameof(BaseUrl), typeof(string), typeof(XamlMarkdown), new PropertyMetadata(null));
        public static readonly DependencyProperty HyperlinkCommandProperty =
            DependencyProperty.Register(nameof(HyperlinkCommand), typeof(ICommand), typeof(XamlMarkdown), new PropertyMetadata(null));
        public static readonly DependencyProperty IsStrictBoldItalicProperty =
            DependencyProperty.Register(nameof(IsStrictBoldItalic), typeof(bool), typeof(XamlMarkdown), new PropertyMetadata(BooleanBoxes._falseBox));
        public static ComponentResourceKey CodeStyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(CodeStyleKey));
        public static ComponentResourceKey DocumentStyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(DocumentStyleKey));
        public static ComponentResourceKey Heading1StyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(Heading1StyleKey));
        public static ComponentResourceKey Heading2StyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(Heading2StyleKey));
        public static ComponentResourceKey Heading3StyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(Heading3StyleKey));
        public static ComponentResourceKey Heading4StyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(Heading4StyleKey));
        public static ComponentResourceKey ImageStyleKey { get; } = new ComponentResourceKey(typeof(XamlMarkdown), nameof(ImageStyleKey));

        private readonly FrameworkElement _resourcesProvider;
        private int _listLevel;
        private Style _codeStyle;
        private Style _documentStyle;
        private Style _heading1Style;
        private Style _heading2Style;
        private Style _heading3Style;
        private Style _heading4Style;
        private Style _imageStyle;

        private Style CodeStyle => _codeStyle is null ? _codeStyle = TryFindStyle(CodeStyleKey) : _codeStyle;
        private Style DocumentStyle => _documentStyle is null ? _documentStyle = TryFindStyle(DocumentStyleKey) : _documentStyle;
        private Style Heading1Style => _heading1Style is null ? _heading1Style = TryFindStyle(Heading1StyleKey) : _heading1Style;
        private Style Heading2Style => _heading2Style is null ? _heading2Style = TryFindStyle(Heading2StyleKey) : _heading2Style;
        private Style Heading3Style => _heading3Style is null ? _heading3Style = TryFindStyle(Heading3StyleKey) : _heading3Style;
        private Style Heading4Style => _heading4Style is null ? _heading4Style = TryFindStyle(Heading4StyleKey) : _heading4Style;
        private Style ImageStyle => _imageStyle is null ? _imageStyle = TryFindStyle(ImageStyleKey) : _imageStyle;

        public string BaseUrl { get => (string)GetValue(BaseUrlProperty); set => SetValue(BaseUrlProperty, value); }
        public ICommand HyperlinkCommand { get => (ICommand)GetValue(HyperlinkCommandProperty); set => SetValue(HyperlinkCommandProperty, value); }
        public bool IsStrictBoldItalic { get => (bool)GetValue(IsStrictBoldItalicProperty); set => SetValue(IsStrictBoldItalicProperty, value); }

        public XamlMarkdown()
        {
            HyperlinkCommand = NavigationCommands.GoToPage;
        }

        public XamlMarkdown(FrameworkElement resourcesProvider) : this()
        {
            _resourcesProvider = resourcesProvider ?? throw new ArgumentNullException(nameof(resourcesProvider));
        }

        private static string RepeatString(string text, int count)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            StringBuilder sb = new StringBuilder(text.Length * count);
            for (int idx = 0; idx < count; ++idx)
            {
                sb.Append(text);
            }
            return sb.ToString();
        }

        private static string GetNestedBracketsPattern()
        {
            return _nestedBracketsPattern is null ? _nestedBracketsPattern = RepeatString(@"(?>[^\[\]]+|\[", _maxNestDepth) + RepeatString(@"\])*", _maxNestDepth) : _nestedBracketsPattern;
        }

        private static string GetNestedParensPattern()
        {
            return _nestedParensPattern is null ? _nestedParensPattern = RepeatString(@"(?>[^()\s]+|\(", _maxNestDepth) + RepeatString(@"\))*", _maxNestDepth) : _nestedParensPattern;
        }

        private static T Create<T, TContent>(IEnumerable<TContent> content) where T : IAddChild, new()
        {
            T result = new T();
            foreach (TContent c in content)
            {
                result.AddChild(c);
            }
            return result;
        }

        private Style TryFindStyle(object resourceKey)
        {
            return _resourcesProvider?.TryFindResource(resourceKey) as Style;
        }

        private string Normalize(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            StringBuilder sb = new StringBuilder(text.Length);
            StringBuilder line = new StringBuilder();
            bool isValid = false;
            for (int idx = 0; idx < text.Length; ++idx)
            {
                switch (text[idx])
                {
                    case '\n':
                        if (isValid)
                        {
                            sb.Append(line);
                        }
                        sb.Append('\n');
                        line.Length = 0;
                        isValid = false;
                        break;
                    case '\r':
                        if ((idx < text.Length + 1) && (text[idx + 1] != '\n'))
                        {
                            if (isValid)
                            {
                                sb.Append(line);
                            }
                            sb.Append('\n');
                            line.Length = 0;
                            isValid = false;
                        }
                        break;
                    case '\t':
                        int width = _tabWidth - (line.Length % _tabWidth);
                        for (int idy = 0; idy < width; ++idy)
                        {
                            line.Append(' ');
                        }
                        break;
                    case '\x1A':
                        break;
                    default:
                        if (!isValid && text[idx] != ' ')
                        {
                            isValid = true;
                        }
                        line.Append(text[idx]);
                        break;
                }
            }
            if (isValid)
            {
                sb.Append(line);
            }
            sb.Append("\n\n\n");
            return sb.ToString();
        }

        private IEnumerable<T> Evaluate<T>(string text, Regex expression, Func<Match, T> build, Func<string, IEnumerable<T>> rest)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            MatchCollection matches = expression.Matches(text);
            int index = 0;
            foreach (Match match in matches)
            {
                if (match.Index > index)
                {
                    string prefix = text.Substring(index, match.Index - index);
                    foreach (T t in rest(prefix))
                    {
                        yield return t;
                    }
                }
                yield return build(match);
                index = match.Index + match.Length;
            }
            if (index < text.Length)
            {
                string suffix = text.Substring(index);
                foreach (T t in rest(suffix))
                {
                    yield return t;
                }
            }
        }

        private IEnumerable<Inline> DoText(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            string t = _eoln.Replace(text, " ");
            yield return new Run(t);
        }

        private IEnumerable<Inline> DoHardBreaks(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _hardBreak, x => new LineBreak(), defaultHandler);
        }

        private Inline BoldEvaluator(Match match, int contentGroup)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            return Create<Bold, Inline>(RunSpanGamut(match.Groups[contentGroup].Value));
        }

        private Inline ItalicEvaluator(Match match, int contentGroup)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            return Create<Italic, Inline>(RunSpanGamut(match.Groups[contentGroup].Value));
        }

        private IEnumerable<Inline> DoItalicsAndBold(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            if (IsStrictBoldItalic)
            {
                return Evaluate(text, _strictBold, m => BoldEvaluator(m, 3), x => Evaluate(x, _strictItalic, m => ItalicEvaluator(m, 3), defaultHandler));
            }
            return Evaluate(text, _bold, m => BoldEvaluator(m, 2), x => Evaluate(x, _italic, m => ItalicEvaluator(m, 2), defaultHandler));
        }

        private Inline AnchorInlineEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string linkText = match.Groups[2].Value;
            string url = match.Groups[3].Value;
            string title = match.Groups[6].Value;
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && !(Path.IsPathRooted(url) || !url.StartsWith("/") || !url.StartsWith("\\")))
            {
                url = (BaseUrl ?? string.Empty) + url;
            }
            Hyperlink result = Create<Hyperlink, Inline>(RunSpanGamut(linkText));
            result.Click += (_, __) => HyperlinkCommand.Execute(url);
            return result;
        }

        private IEnumerable<Inline> DoAnchors(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _anchorInline, AnchorInlineEvaluator, defaultHandler);
        }

        private Inline CodeSpanEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string span = match.Groups[2].Value;
            span = Regex.Replace(span, @"^[ ]*", "");
            span = Regex.Replace(span, @"[ ]*$", "");
            Inline result = new Run(span);
            if (!(CodeStyle is null))
            {
                try
                {
                    result.Style = CodeStyle;
                }
                catch (InvalidOperationException)
                {
                }
            }
            return result;
        }

        private Inline ImageTag(string url, string altText, string title)
        {
            Image result = new Image();
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && (!Path.IsPathRooted(url) || !url.StartsWith("/") || !url.StartsWith("\\")))
                {
                    url = (BaseUrl ?? string.Empty) + url;
                }
                result.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            }
            catch (IOException)
            {
                return new Run($"Error loading '{url}'") { Foreground = Brushes.Red };
            }
            catch (ExternalException)
            {
                return new Run($"Error loading '{url}'") { Foreground = Brushes.Red };
            }
            if (!string.IsNullOrEmpty(title))
            {
                result.ToolTip = Create<TextBlock, Inline>(RunSpanGamut(title));
            }
            if (!(ImageStyle is null))
            {
                try
                {
                    result.Style = ImageStyle;
                }
                catch (InvalidOperationException)
                {
                }
            }
            return new InlineUIContainer(result);
        }

        private Inline ImageInlineEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string altText = match.Groups[2].Value;
            string url = match.Groups[3].Value;
            string title = match.Groups[6].Value;
            if (url.StartsWith("<") && url.EndsWith(">"))
            {
                url = url.Substring(1, url.Length - 2);
            }
            return ImageTag(url, altText, title);
        }

        private Inline HtmlImageInlineEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string url = match.Groups[3].Value;
            if (url.StartsWith("<") && url.EndsWith(">"))
            {
                url = url.Substring(1, url.Length - 2);
            }
            return ImageTag(url, null, null);
        }

        private IEnumerable<Inline> DoImages(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _htmlImageInline, HtmlImageInlineEvaluator, s => Evaluate(s, _imageInline, ImageInlineEvaluator, defaultHandler));
        }

        private IEnumerable<Inline> DoCodeSpans(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _codeSpan, CodeSpanEvaluator, defaultHandler);
        }

        private IEnumerable<Inline> RunSpanGamut(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return DoCodeSpans(text, x => DoImages(x, y => DoAnchors(y, z => DoItalicsAndBold(z, w => DoHardBreaks(w, DoText)))));
        }

        private IEnumerable<Block> FormParagraphs(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            string[] paragraphs = _newLinesMultiple.Split(_newLinesLeadingTrailing.Replace(text, ""));
            foreach (string paragraph in paragraphs)
            {
                yield return Create<Paragraph, Inline>(RunSpanGamut(paragraph));
            }
        }

        private ListItem ListItemEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string item = match.Groups[4].Value;
            string leadingLine = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(leadingLine) || Regex.IsMatch(item, @"\n{2,}"))
            {
                return Create<ListItem, Block>(RunBlockGamut(item));
            }
            return Create<ListItem, Block>(RunBlockGamut(item));
        }

        private IEnumerable<ListItem> ProcessListItems(string list, string marker)
        {
            ++_listLevel;
            try
            {
                list = Regex.Replace(list, @"\n{2,}\z", "\n");
                string pattern = string.Format(@"(\n)?(^[ ]*)({0})[ ]+((?s:.+?)(\n{{1,2}}))(?=\n*(\z|\2({0})[ ]+))", marker);
                Regex regex = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection matches = regex.Matches(list);
                foreach (Match match in matches)
                {
                    yield return ListItemEvaluator(match);
                }
            }
            finally
            {
                --_listLevel;
            }
        }

        private Block ListEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string list = match.Groups[1].Value;
            string listType = Regex.IsMatch(match.Groups[3].Value, _markerUnorderedList) ? "ul" : "ol";
            list = Regex.Replace(list, @"\n{2,}", "\n\n\n");
            List result = Create<List, ListItem>(ProcessListItems(list, listType == "ul" ? _markerUnorderedList : _markerOrderedList));
            result.MarkerStyle = listType == "ul" ? TextMarkerStyle.Disc : TextMarkerStyle.Decimal;
            return result;
        }

        private IEnumerable<Block> DoLists(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _listLevel > 0 ? _listNested : _listTopLevel, ListEvaluator, defaultHandler);
        }

        private IEnumerable<Block> DoHorizontalRules(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _horizontalRules, RuleEvaluator, defaultHandler);
        }

        private Block RuleEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            return new BlockUIContainer(new System.Windows.Shapes.Line { X2 = 1, StrokeThickness = 1.0 });
        }

        private Block CreateHeader(int level, IEnumerable<Inline> content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
            Paragraph result = Create<Paragraph, Inline>(content);
            try
            {
                switch (level)
                {
                    case 1:
                        if (!(Heading1Style is null))
                        {
                            result.Style = Heading1Style;
                        }
                        break;
                    case 2:
                        if (!(Heading2Style is null))
                        {
                            result.Style = Heading2Style;
                        }
                        break;
                    case 3:
                        if (!(Heading3Style is null))
                        {
                            result.Style = Heading3Style;
                        }
                        break;
                    case 4:
                        if (!(Heading4Style is null))
                        {
                            result.Style = Heading4Style;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (InvalidOperationException)
            {
            }
            return result;
        }

        private Block AtxHeaderEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string header = match.Groups[2].Value;
            int level = match.Groups[1].Value.Length;
            return CreateHeader(level, RunSpanGamut(header));
        }

        private Block SetextHeaderEvaluator(Match match)
        {
            if (match is null) throw new ArgumentNullException(nameof(match));
            string header = match.Groups[1].Value;
            int level = match.Groups[2].Value.StartsWith("=") ? 1 : 2;
            return CreateHeader(level, RunSpanGamut(header.Trim()));
        }


        private IEnumerable<Block> DoHeaders(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return Evaluate(text, _headerSetext, SetextHeaderEvaluator, x => Evaluate(x, _headerAtx, AtxHeaderEvaluator, defaultHandler));
        }

        private IEnumerable<Block> RunBlockGamut(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            return DoHeaders(text, x => DoHorizontalRules(x, y => DoLists(y, FormParagraphs)));
        }

        public FlowDocument Transform(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            text = Normalize(text);
            FlowDocument result = Create<FlowDocument, Block>(RunBlockGamut(text));
            if (!(DocumentStyle is null))
            {
                try
                {
                    result.Style = DocumentStyle;
                }
                catch (InvalidOperationException)
                {
                }
            }
            return result;
        }
    }
}
