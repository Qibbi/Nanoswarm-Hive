using System;
using System.Globalization;
using System.Windows;

namespace NanoswarmHive.Presentation.ValueConverters
{
    public class VisibleOrCollapsed : AValueConverterBase<VisibleOrCollapsed>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            bool result = ConverterHelper.ConvertToBoolean(value, cultureInfo);
            if ((parameter as bool?) == false)
            {
                result = !result;
            }
            return result ? VisibilityBoxes._visibleBox : VisibilityBoxes._collapsedBox;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            Visibility visibility = (Visibility)value;
            bool result = visibility == Visibility.Visible;
            if ((parameter as bool?) == false)
            {
                result = !result;
            }
            return result.Box();
        }
    }
}
