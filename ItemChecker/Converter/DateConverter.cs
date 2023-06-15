using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString("dd MMM yy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString("dd MMM yy");
        }
    }
}
