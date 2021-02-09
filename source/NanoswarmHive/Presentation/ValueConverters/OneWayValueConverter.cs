using System;
using System.Globalization;
using System.Windows.Data;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public abstract class AOneWayValueConverter<T> : AValueConverterBase<T> where T : class, IValueConverter, new()
    {
        public sealed override object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotSupportedException($"{nameof(ConvertBack)} is not supported.");
        }
    }
}
