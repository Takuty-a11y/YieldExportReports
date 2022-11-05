using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace YieldExportReports.Report.ReportFields
{
    public sealed class ReportFieldCollection : IEnumerable<ReportField>
    {
        private readonly List<ReportField> resources = new();
        public ReportField this[int index]
        {
            get { return resources[index]; }
            set { resources[index] = value; }
        }
        public void Add(ReportField item)
        {
            resources.Add(item);
        }
        public void Delete(ReportField item)
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

        public List<ReportField> ToList()
        {
            return resources;
        }

        public IEnumerator<ReportField> GetEnumerator()
        {
            foreach (var resource in resources)
            {
                yield return resource;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
