using System;
using System.Globalization;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public class ObjectToBool : AOneWayValueConverter<ObjectToBool>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return (!(value is null)).Box();
        }
    }
}
