using Nanocore.Core.Language;

namespace Nanocore.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Translate(this string str)
        {
            return TranslationManager.Current.GetString(str);
        }
    }
}
