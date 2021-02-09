using Nanocore.Core.Serialization;
using System;
using System.IO;

namespace Nanocore.Core.Language
{
    public class TranslationManager
    {
        public static TranslationManager Current { get; } = new TranslationManager();

        private Csf _csf = Csf.Empty;

        public event EventHandler LanguageChanged;

        private TranslationManager()
        {
        }

        public void LoadStrings(string file)
        {
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _csf = new Csf(new BinarySerializationReader(stream));
            }
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public string GetString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            _csf.TryGetString(str, out string result);
            return result;
        }
    }
}
