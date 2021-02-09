using Nanocore.Core.Diagnostics;
using Nanocore.Core.Serialization;
using System;
using System.Collections.Generic;

namespace Nanocore.Core.Language
{
    public enum LanguageType
    {
        [Display("en")] English,
        [Display("es")] Spanish,
        [Display("de")] German = 3,
        [Display("fr")] French,
        [Display("it")] Italian,
        [Display("nl")] Dutch,
        [Display("sv")] Swedish = 8,
        [Display("pl")] Polish = 11,
        [Display("ko")] Korean = 13,
        [Display("hu")] Hungarian = 15,
        [Display("ru")] Russian = 17,
        [Display("cs")] Czech = 19,
        [Display("zh-Hans")] ChineseSimplified = 21, // zh-CN
        [Display("zh-Hant")] ChineseTraditional, // zh-TW
        [Display("th-TH")] Thai,
        German16 = 100
    }

    public sealed class Csf
    {
        private const string _missing = "MISSING: '{0}'";
        private const int _magicCsf = 0x43534620;
        private const int _magicLabel = 0x4C424C20;
        private const int _magicLString = 0x53545220;
        private const int _magicLWideString = 0x53545257;

        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(Csf), "Handles string files");
        public static Csf Empty { get; } = new Csf();

        private readonly Dictionary<string, Dictionary<string, string>> _strings;

        public int Version { get; }
        public int NumLabels { get; }
        public int NumStrings { get; }
        public LanguageType LanguageType { get; }

        private Csf()
        {
            _strings = new Dictionary<string, Dictionary<string, string>> { { "GUI", new Dictionary<string, string> { { "Ok", "Ok" } } } };
        }

        public Csf(ASerializationStream stream)
        {
            _strings = new Dictionary<string, Dictionary<string, string>>();
            int magic = stream.ReadInt32();
            if (magic != _magicCsf) throw new InvalidOperationException("Error parsing csf (Magic: CSF expected).");
            Version = stream.ReadInt32();
            NumLabels = stream.ReadInt32();
            NumStrings = stream.ReadInt32();
            stream.ReadInt32(); // reserved
            if (NumLabels != NumStrings) throw new NotSupportedException("Csf substrings are not supported.");
            LanguageType = (LanguageType)stream.ReadInt32();
            string label;
            string str;
            int colonIdx;
            string categoryLabel;
            for (int idx = 0; idx < NumStrings; ++idx)
            {
                magic = stream.ReadInt32();
                if (magic != _magicLabel) throw new InvalidOperationException("Error parsing csf (Magic: LBL expected).");
                if (stream.ReadInt32() != 1) throw new InvalidOperationException("String has multiple substrings (this should not even happen).");
                label = stream.ReadString(stream.ReadInt32());
                magic = stream.ReadInt32();
                if (magic != _magicLString && magic != _magicLWideString) throw new InvalidOperationException("Error parsing csf (Magic: STR/STRW expected).");
                str = stream.ReadUnicodeStringNegate(stream.ReadInt32());
                if (magic == _magicLWideString)
                {
                    str += stream.ReadString(stream.ReadInt32());
                }
                colonIdx = label.IndexOf(':');
                if (colonIdx == -1)
                {
                    categoryLabel = string.Empty;
                }
                else
                {
                    categoryLabel = label.Substring(0, colonIdx);
                }
                if (!_strings.TryGetValue(categoryLabel, out Dictionary<string, string> category))
                {
                    category = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _strings.Add(categoryLabel, category);
                }
                label = label.Substring(colonIdx + 1);
                if (category.ContainsKey(label))
                {
                    _tracer.TraceWarning($"String duplication: '{categoryLabel}:{label}' -> '{category[label]}', new value: '{str}'");
                    // throw new InvalidOperationException("String duplication.");
                }
                else
                {
                    category.Add(label, str);
                }
            }
        }

        public bool TryGetString(string str, out string result)
        {
            int colonIdx = str.IndexOf(':');
            string label = string.Empty;
            if (colonIdx != -1)
            {
                label = str.Substring(0, colonIdx);
            }
            if (_strings.TryGetValue(label, out Dictionary<string, string> category) && category.TryGetValue(str.Substring(colonIdx + 1), out result))
            {
                return true;
            }
            result = string.Format(_missing, str);
            return false;
        }
    }
}
