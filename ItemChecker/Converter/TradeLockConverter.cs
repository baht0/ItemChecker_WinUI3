using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    internal class TradeLockConverter : IValueConverter
    {
        static string ToReadable(object value) => ((DateTime)value).ToString("dd MMM yy");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime)
                return value;
            int days = (int)Math.Floor((((DateTime)value) - DateTime.Now).TotalDays);
            days = days > 0 ? days : days * (-1);
            return (DateTime)value == new DateTime() ? "Free" : $"{ToReadable(value)} | {days} Days";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime)
                return value;
            int days = (int)Math.Floor((((DateTime)value) - DateTime.Now).TotalDays);
            days = days > 0 ? days : days * (-1);
            return (DateTime)value == new DateTime() ? "Free" : $"{ToReadable(value)} | {days} Days";
        }
    }
}
