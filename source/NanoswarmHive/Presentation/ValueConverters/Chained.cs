using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public class Chained : MarkupExtension, IValueConverter
    {
        public const int MaxConverterCount = 2;

        private readonly IValueConverter[] _converters = new IValueConverter[MaxConverterCount];
        private readonly Type[] _converterTargetTypes = new Type[MaxConverterCount];
        private readonly object[] _converterParameters = new object[MaxConverterCount];

        public Chained(IValueConverter converter0, IValueConverter converter1)
        {
            _converters[0] = converter0;
            _converters[1] = converter1;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            object result = value;
            for (int idx = 0; idx < MaxConverterCount; ++idx)
            {
                object input = result;
                if (_converters[idx] is null)
                {
                    break;
                }
                Type type = _converterTargetTypes[idx] ?? ((idx == MaxConverterCount - 1) || _converters[idx + 1] is null ? targetType : typeof(object));
                result = _converters[idx].Convert(input, type, _converterParameters[idx], cultureInfo);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotSupportedException();
        }
    }
}
