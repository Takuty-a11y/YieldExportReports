using System.Windows;
using System.Windows.Input;
using YieldExportReports.Database.DBResources;
using YieldExportReports.Dock;
using YieldExportReports.Utils;
using YieldExportReports.ViewModels.Base;
using YieldExportReports.Views.Login;

namespace YieldExportReports.ViewModels.Login
{
    public class LoginWindowViewModel : DialogViewModel
    {
        private readonly ShowWindowService<DBSettingWindow, DBSettingWindowViewModel> m_settingWindowService;

        public LoginWindowViewModel()
        {
            m_settingWindowService =
                new ShowWindowService<DBSettingWindow, DBSettingWindowViewModel>();

            GetDBSettings();
        }

        /// <summary>
        /// DB設定データ
        /// </summary>
        public DBResourceCollection DbResourceCollection
        {
            get { return m_dbResourceCollection; }
            set
            {
                if (value != m_dbResourceCollection)
                {
                    m_dbResourceCollection = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DBResourceCollection m_dbResourceCollection = new();

        /// <summary>
        /// 選択中のDB設定
        /// </summary>
        public DBResource? SelectedDBResource
        {
            get { return m_selectedDBResource; }
            set
            {
                if (value != m_selectedDBResource)
                {
                    m_selectedDBResource = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(OKEnabled));
                }
            }
        }
        private DBResource? m_selectedDBResource;

        /// <summary>
        /// 設定値追加コマンド
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                if (m_addCommand == null)
                {
                    m_addCommand = new RelayCommand(() =>
                    {
                        m_settingWindowService.Owner = Owner;
                        var vm = new DBSettingWindowViewModel();
                        if (m_settingWindowService.ShowDialog(vm) == true)
                        {
                            m_dbResourceCollection.Add(vm.TargetDBResource);
                            DBResourceCollectionFactory.Serialize(m_dbResourceCollection);
                        }
                        GetDBSettings();
                    });
                }
                return m_addCommand;
            }
        }
        private RelayCommand? m_addCommand;

        /// <summary>
        /// 設定値編集コマンド
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                if (m_editCommand == null)
                {
                    m_editCommand = new RelayCommand(() =>
                    {
                        if (m_selectedDBResource == null)
                        { return; }

                        m_settingWindowService.Owner = Owner;
                        var vm = new DBSettingWindowViewModel(m_selectedDBResource);
                        if (m_settingWindowService.ShowDialog(vm) == true)
                        {
                            m_dbResourceCollection.Delete(m_selectedDBResource);
                            m_dbResourceCollection.Add(vm.TargetDBResource);
                            DBResourceCollectionFactory.Serialize(m_dbResourceCollection);
                        }
                        GetDBSettings();
                    });
                }
                return m_editCommand;
            }
        }
        private RelayCommand? m_editCommand;

        /// <summary>
        /// 設定値削除コマンド
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (m_deleteCommand == null)
                {
                    m_deleteCommand = new RelayCommand(() =>
                    {
                        if (m_selectedDBResource == null)
                        { return; }

                        m_dbResourceCollection.Delete(m_selectedDBResource);
                        DBResourceCollectionFactory.Serialize(m_dbResourceCollection);

                        GetDBSettings();
                    });
                }
                return m_deleteCommand;
            }
        }
        private RelayCommand? m_deleteCommand;

        /// <summary>
        /// OKボタン有効無効
        /// </summary>
        public bool OKEnabled
        {
            get { return m_selectedDBResource != null; }
        }

        /// <summary>
        /// OKボタンコマンド処理
        /// </summary>
        /// <returns>実行可否</returns>
        protected override bool OKCommandMethod()
        {
            if (m_selectedDBResource == null)
            {
                MessageBox.Show(
                    "設定が正しく選択されていません",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            m_selectedDBResource.IsConnected = true;
            DBResourceCollectionFactory.Serialize(m_dbResourceCollection);
            return true;
        }

        /// <summary>
        /// ファイルからDB設定を取得します
        /// </summary>
        private void GetDBSettings()
            => DbResourceCollection = DBResourceCollectionFactory.GetDBResourceOnLogin();
    }
}
