using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace ItemChecker.Converter
{
    class BooleanReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
