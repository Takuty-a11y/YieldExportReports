using System.Data;
using System.Linq;
using System.Windows.Controls;
using YieldExportReports.ViewModels.Dock;

namespace YieldExportReports.Views.Dock
{
    /// <summary>
    /// DataGridViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class DataGridViewControl : UserControl
    {
        public DataGridViewControl()
        {
            InitializeComponent();
        }
    }

    public class MyDataGrid : DataGrid
    {
        public static string RowIndexName = DataGridViewTool.HeaderRowID;
        public MyDataGrid()
        {
            this.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue is not DataTable dt) { return; }

                if (!dt.Columns.Contains(RowIndexName))
                { dt.Columns.Add(RowIndexName, typeof(int)); }

                foreach (var cls in dt.AsEnumerable().Select((dr, index) => new { dr, index }))
                {
                    cls.dr[RowIndexName] = cls.index + 1;
                }
            };
            this.AutoGeneratingColumn += (sender, e) =>
            {
                if (e.PropertyName == RowIndexName)
                    e.Cancel = true;
            };
        }
        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            base.OnSorting(eventArgs);
            foreach (var index in Enumerable.Range(0, this.Items.Count))
            {
                if (this.Items[index] is not DataRowView r) { return; }
                r[RowIndexName] = index + 1;
            }
        }
    }
}
