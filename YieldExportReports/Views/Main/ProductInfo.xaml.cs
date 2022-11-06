using MahApps.Metro.Controls;
using System;
using System.Reflection;
using System.Windows.Navigation;
using YieldExportReports.Utils;

namespace YieldExportReports.Views.Main
{
    /// <summary>
    /// ProductInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class ProductInfo : MetroWindow
    {
        public ProductInfo()
        {
            InitializeComponent();

            InitializeInfo();
        }

        private void InitializeInfo()
        {
            var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
            lblVersion.Content = $"Version：{version}";

            var nowYear = DateTime.Now.Year;
            lblCopyright.Content = $"Copyright Takkuto Otsuka 2022-{nowYear} All Rights Reserved";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            AppHelper.OpenLinkProcess(e.Uri.AbsoluteUri);
        }
    }
}
