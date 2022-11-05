using System;
using System.Globalization;
using System.Windows.Data;

namespace YieldExportReports.Dock
{
    public class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToolContent content)
            {
                if (content.IsDocument)
                    return value;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToolContent content)
            {
                if (content.IsDocument)
                    return value;
            }

            return Binding.DoNothing;
        }
    }
}
