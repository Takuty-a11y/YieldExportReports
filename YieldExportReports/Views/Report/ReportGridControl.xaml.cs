using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YieldExportReports.Report.ReportSettings;

namespace YieldExportReports.Views.Report
{
    /// <summary>
    /// ReportGridControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ReportGridControl : UserControl
    {
        public ReportGridControl()
        {
            InitializeComponent();
        }
        public List<ReportSetting> ItemSource
        {
            get { return (List<ReportSetting>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public ReportSetting SelectedItem
        {
            get { return (ReportSetting)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public Visibility CellVisible
        {
            get { return (Visibility)GetValue(CellVisibleProperty); }
            set { SetValue(CellVisibleProperty, value); }
        }
        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty
            = DependencyProperty.Register(
                nameof(ItemSource),
                typeof(List<ReportSetting>),
                typeof(ReportGridControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(ReportSetting),
                typeof(ReportGridControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CellVisibleProperty
            = DependencyProperty.Register(
                nameof(CellVisible),
                typeof(Visibility),
                typeof(ReportGridControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(
                nameof(DeleteCommand),
                typeof(ICommand),
                typeof(ReportGridControl),
                new PropertyMetadata(null));
    }

    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register(
                nameof(Data),
                typeof(object),
                typeof(BindingProxy));
    }
}
