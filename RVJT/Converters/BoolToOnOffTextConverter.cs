using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RVJT.Converters;

public class BoolToOnOffTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "On" : "Off";
        return "Off"; 
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
            return stringValue.Equals("On", StringComparison.OrdinalIgnoreCase);
        return false;
    }
}