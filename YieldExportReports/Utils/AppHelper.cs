using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YieldExportReports.Utils
{
    public static class AppHelper
    {
        public static string AppDirectory
        {
            get 
            {
                return
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                        ?? @"C:\";
            }
        }

        public static PropertyInfo[] GetMyProperties<T>()
        {
            var typClass = typeof(T);
            return typClass.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);
        }

        public static async void OpenLinkProcess(string? link)
        {
            try
            {
                await Task.Run(() => 
                {
                     Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = link ?? throw new ArgumentNullException(nameof(link)),
                            UseShellExecute = true,
                        }
                    );
                });
            }
            catch (Exception e)
            {
                var sbMessage = new StringBuilder();
                sbMessage.AppendLine("リンクを開けませんでした。");
                sbMessage.AppendLine(link);
                sbMessage.AppendLine();
                sbMessage.AppendLine(e.Message);
                MessageBox.Show(
                    sbMessage.ToString(), 
                    "リンクエラー", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
    }
}
