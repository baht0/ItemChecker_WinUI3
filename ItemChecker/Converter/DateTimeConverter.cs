using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString("dd MMM yy HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString("dd MMM yy HH:mm");
        }
    }
}
