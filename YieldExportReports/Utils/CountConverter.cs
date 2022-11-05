﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace YieldExportReports.Utils
{
    [ValueConversion(typeof(int), typeof(int))]
    public class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not int ? value : (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
