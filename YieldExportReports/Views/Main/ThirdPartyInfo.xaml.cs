using MahApps.Metro.Controls;
using System.IO;
using System.Reflection;
using System.Windows.Navigation;
using YieldExportReports.Utils;

namespace YieldExportReports.Views.Main
{
    /// <summary>
    /// ThirdPartyInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class ThirdPartyInfo : MetroWindow
    {
        public ThirdPartyInfo()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            AppHelper.OpenLinkProcess(e.Uri.AbsoluteUri);
        }

        private void Hyperlink_RequestNavigate_1(object sender, RequestNavigateEventArgs e)
        {
            var path = Path.Combine(AppHelper.AppDirectory, e.Uri.OriginalString);
            AppHelper.OpenLinkProcess(path);
        }
    }
}
