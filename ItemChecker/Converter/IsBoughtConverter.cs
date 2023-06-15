using ItemChecker.Models.Rare;
using Microsoft.UI.Xaml.Data;
using System;

namespace ItemChecker.Converter
{
    internal class IsBoughtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((string)parameter == "0")
                return (BuyStatus)value switch
                {
                    BuyStatus.OK => false,
                    _ => true
                };
            else
                return (BuyStatus)value switch
                {
                    BuyStatus.OK => "Successfully",
                    BuyStatus.Error => "Not purchased",
                    _ => string.Empty
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((int)parameter == 1)
                return (BuyStatus)value switch
                {
                    BuyStatus.OK => false,
                    _ => true
                };
            else
                return (BuyStatus)value switch
                {
                    BuyStatus.OK => "Successfully",
                    BuyStatus.Error => "Not purchased",
                    _ => string.Empty
                };
        }
    }
}
