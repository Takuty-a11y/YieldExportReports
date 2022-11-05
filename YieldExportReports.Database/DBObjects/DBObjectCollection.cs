using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace YieldExportReports.Database.DBObjects
{
    public class DBObjectCollection : IEnumerable<DBObject>
    {
        private readonly List<DBObject> resources = new();
        public DBObject this[int index]
        {
            get { return resources[index]; }
            set { resources[index] = value; }
        }
        public void Add(DBObject item)
        {
            resources.Add(item);
        }
        public void Delete(DBObject item)
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

        public IEnumerator<DBObject> GetEnumerator()
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
