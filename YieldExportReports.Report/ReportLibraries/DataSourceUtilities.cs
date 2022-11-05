using System;

namespace YieldExportReports.Report.ReportLibraries
{
    public static class DataSourceUtilities
    {
        /// <summary>
        /// データ型ごとに文字列に変換します
        /// </summary>
        /// <param name="type">データ型</param>
        /// <param name="cellValue">値</param>
        /// <returns>文字列</returns>
        internal static string DataTypeString(Type? type, object cellValue)
        {
            if (IsBool(type, cellValue, out bool bValue))
            {
                if (bValue)
                { return bool.TrueString; }
                else
                { return bool.FalseString; }
            }
            if (IsDatatime(type, cellValue, out DateTime dtValue))
            {
                return dtValue.ToLongDateString();
            }

            return cellValue?.ToString() ?? string.Empty;
        }

        private static bool IsBool(Type? type, object value, out bool result)
        {
            result = false;
            if (type != typeof(bool))
                return false;

            if (!bool.TryParse(value?.ToString(), out result))
                return false;

            return true;
        }

        private static bool IsDatatime(Type? type, object value, out DateTime result)
        {
            result = new DateTime();
            if (type != typeof(DateTime))
                return false;

            if (!DateTime.TryParse(value?.ToString(), out result))
                return false;

            return true;
        }
    }
}
