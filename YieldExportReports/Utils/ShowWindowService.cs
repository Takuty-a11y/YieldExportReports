using System.Windows;
using YieldExportReports.ViewModels.Base;

namespace YieldExportReports.Utils
{
    public class ShowWindowService<TWindow, TViewModel> : IShowWindowService<TViewModel>
        where TWindow : Window, new()
        where TViewModel : DialogViewModel
    {
        public Window? Owner { get; set; }

        public bool? ShowDialog(TViewModel viewModel)
        {
            var dlg = new TWindow()
            {
                Owner = this.Owner,
                DataContext = viewModel,
            };

            viewModel.Owner = dlg;

            return dlg.ShowDialog();
        }

        public void Show(TViewModel viewModel)
        {
            var dlg = new TWindow()
            {
                Owner = this.Owner,
                DataContext = viewModel,
            };

            dlg.Show();
        }
    }
}
