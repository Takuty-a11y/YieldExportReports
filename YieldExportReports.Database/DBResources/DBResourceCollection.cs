using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace YieldExportReports.Database.DBResources
{
    [Serializable]
    public class DBResourceCollection : IEnumerable<DBResource>
    {
        private readonly List<DBResource> resources = new();

        public DBResource this[int index]
        {
            get { return resources[index]; }
            set { resources[index] = value; }
        }
        public void Add(DBResource? item)
        {
            if (item != null)
            { resources.Add(item); }
        }
        public void Delete(DBResource item)
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

        public IEnumerator<DBResource> GetEnumerator()
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
