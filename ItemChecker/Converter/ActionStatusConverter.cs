using Microsoft.UI.Xaml.Data;
using System;
using static ItemChecker.Models.StaticModels.BaseConfig;

namespace ItemChecker.Converter
{
    internal class ActionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((string)parameter == "0")
                return (ActionStatus)value switch
                {
                    ActionStatus.OK => false,
                    _ => true
                };
            else
                return (ActionStatus)value switch
                {
                    ActionStatus.OK => "Successfully",
                    ActionStatus.Error => "Something went wrong",
                    _ => string.Empty
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((string)parameter == "0")
                return (ActionStatus)value switch
                {
                    ActionStatus.OK => false,
                    _ => true
                };
            else
                return (ActionStatus)value switch
                {
                    ActionStatus.OK => "Successfully",
                    ActionStatus.Error => "Something went wrong",
                    _ => string.Empty
                };
        }
    }
}
