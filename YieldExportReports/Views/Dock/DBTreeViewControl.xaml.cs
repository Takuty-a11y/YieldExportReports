using System.Windows.Controls;
using System.Windows.Input;

namespace YieldExportReports.Views.Dock
{
    /// <summary>
    /// DBTreeViewControl.xaml の相互作用ロジック
    /// </summary>
    public partial class DBTreeViewControl : UserControl
    {
        public DBTreeViewControl()
        {
            InitializeComponent();
        }

        private void TvDBData_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                item.IsSelected = true;
                e.Handled = true;
            }
        }
    }
}
