using System.Reflection;

namespace YieldExportReports.Utils
{
    public static class AppHelper
    {
        public static PropertyInfo[] GetMyProperties<T>()
        {
            var typClass = typeof(T);
            return typClass.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
        }
    }
}
