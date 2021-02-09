using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public abstract class AValueConverterBase<T> : MarkupExtension, IValueConverter where T : class, IValueConverter, new()
    {
        private static T _valueConverterInstance;

        protected AValueConverterBase()
        {
            if (GetType() != typeof(T)) throw new InvalidOperationException("Generic argument mismatch.");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _valueConverterInstance is null ? _valueConverterInstance = new T() : _valueConverterInstance;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo);
    }
}
