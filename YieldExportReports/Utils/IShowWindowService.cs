namespace YieldExportReports.Utils
{
    public interface IShowWindowService<TViewModel>
    {
        bool? ShowDialog(TViewModel viewModel);
    }
}
