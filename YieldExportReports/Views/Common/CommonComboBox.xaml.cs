using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YieldExportReports.Views.Common
{
    /// <summary>
    /// CommonComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class CommonComboBox : UserControl
    {
        public CommonComboBox()
        {
            InitializeComponent();
        }

        public IEnumerable ComboItemSource
        {
            get { return (IEnumerable)GetValue(ComboItemSourceProperty); }
            set { SetValue(ComboItemSourceProperty, value); }
        }
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public string SelectedItemName
        {
            get { return (string)GetValue(SelectedItemNameProperty); }
            set { SetValue(SelectedItemNameProperty, value); }
        }
        public bool IsComboEnabled
        {
            get { return (bool)GetValue(SelectedItemNameProperty); }
            set { SetValue(SelectedItemNameProperty, value); }
        }
        public int ComboWidth
        {
            get { return (int)GetValue(ComboWidthProperty); }
            set { SetValue(ComboWidthProperty, value); }
        }
        public ICommand ComboChangeCommand
        {
            get { return (ICommand)GetValue(ComboChangeCommandProperty); }
            set { SetValue(ComboChangeCommandProperty, value); }
        }

        public static readonly DependencyProperty ComboItemSourceProperty
            = DependencyProperty.Register(
                nameof(ComboItemSource),
                typeof(IEnumerable),
                typeof(CommonComboBox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(CommonComboBox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemNameProperty
            = DependencyProperty.Register(
                nameof(SelectedItemName),
                typeof(string),
                typeof(CommonComboBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsComboEnabledProperty
            = DependencyProperty.Register(
                nameof(IsComboEnabled),
                typeof(bool),
                typeof(CommonComboBox),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ComboWidthProperty
            = DependencyProperty.Register(
                nameof(ComboWidth),
                typeof(int),
                typeof(CommonComboBox),
                new PropertyMetadata(310));

        public static readonly DependencyProperty ComboChangeCommandProperty
            = DependencyProperty.Register(
                nameof(ComboChangeCommand),
                typeof(ICommand),
                typeof(CommonComboBox),
                new PropertyMetadata(null));
    }
}
