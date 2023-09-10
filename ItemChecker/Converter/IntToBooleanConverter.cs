using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    internal class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (int)value > 0;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => (int)value > 0;
    }
}
