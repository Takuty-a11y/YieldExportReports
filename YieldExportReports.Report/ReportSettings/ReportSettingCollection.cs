using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace YieldExportReports.Report.ReportSettings
{
    public sealed class ReportSettingCollection : IEnumerable<ReportSetting>
    {
        private readonly List<ReportSetting> resources = new();
        public ReportSetting this[int index]
        {
            get { return resources[index]; }
            set { resources[index] = value; }
        }
        public void Add(ReportSetting item)
        {
            resources.Add(item);
        }
        public void Delete(ReportSetting item)
        {
            try
            {
                resources.Remove(item);
            }
            catch
            {
                MessageBox.Show(
                    "設定が見つかりませんでした。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public IEnumerator<ReportSetting> GetEnumerator()
        {
            foreach (var resource in resources)
            {
                yield return resource;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
