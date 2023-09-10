using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    internal class SteamMarketToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (int)value < 2;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => (int)value < 2;
    }
}
