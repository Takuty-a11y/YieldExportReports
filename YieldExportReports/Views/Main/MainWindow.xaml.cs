using MahApps.Metro.Controls;
using YieldExportReports.Utils;
using YieldExportReports.ViewModels.Login;
using YieldExportReports.ViewModels.Main;
using YieldExportReports.Views.Login;

namespace YieldExportReports.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Title = nameof(YieldExportReports);
            var loginWindowService = new ShowWindowService<LoginWindow, LoginWindowViewModel>()
            {
                Owner = this
            };
            this.DataContext = new MainWindowViewModel(loginWindowService);
        }
    }
}
