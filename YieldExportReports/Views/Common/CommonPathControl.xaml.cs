using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YieldExportReports.Dock;

namespace YieldExportReports.Views.Common
{
    /// <summary>
    /// CommonPathControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CommonPathControl : UserControl
    {
        public CommonPathControl()
        {
            InitializeComponent();

            DeleteFileCommand = new RelayCommand(() =>
            {
                PathText = string.Empty;
            });
        }

        public string PathText
        {
            get { return (string)GetValue(PathTextProperty); }
            set { SetValue(PathTextProperty, value); }
        }
        public int TextWidth
        {
            get { return (int)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }
        public ICommand OpenFileCommand
        {
            get { return (ICommand)GetValue(OpenFileCommandProperty); }
            set { SetValue(OpenFileCommandProperty, value); }
        }
        public ICommand DeleteFileCommand
        {
            get { return (ICommand)GetValue(DeleteFileCommandProperty); }
            set { SetValue(DeleteFileCommandProperty, value); }
        }

        public static readonly DependencyProperty PathTextProperty
            = DependencyProperty.Register(
                nameof(PathText),
                typeof(string),
                typeof(CommonPathControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextWidthProperty
            = DependencyProperty.Register(
                nameof(TextWidth),
                typeof(int),
                typeof(CommonPathControl),
                new PropertyMetadata(400));

        public static readonly DependencyProperty OpenFileCommandProperty
            = DependencyProperty.Register(
                nameof(OpenFileCommand),
                typeof(ICommand),
                typeof(CommonPathControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DeleteFileCommandProperty
            = DependencyProperty.Register(
                nameof(DeleteFileCommand),
                typeof(ICommand),
                typeof(CommonPathControl),
                new PropertyMetadata(null));
    }
}
