using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBResources;
using YieldExportReports.Dock;
using YieldExportReports.ViewModels.Base;

namespace YieldExportReports.ViewModels.Login
{
    public class DBSettingWindowViewModel : DialogViewModel
    {
        public DBSettingWindowViewModel() : this(null) { }
        public DBSettingWindowViewModel(DBResource? dBResource)
        {
            if (dBResource == null)
            {
                SelectedDBType = DBType.MicrosoftSQLServer;
                IsComboEnabled = true;
            }
            else
            {
                SelectedDBType = dBResource.DBType;
                FilePathText = dBResource.SettingFilePath;
                IsComboEnabled = false;
            }
            TargetDBResource = dBResource;
            SwitchUserControlByDBType();
        }

        /// <summary>
        /// 編集対象のDB設定
        /// </summary>
        public DBResource? TargetDBResource
        {
            get { return m_targetDBResource; }
            set
            {
                if (value != m_targetDBResource)
                {
                    m_targetDBResource = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DBResource? m_targetDBResource;

        /// <summary>
        /// DB種別コンボデータ
        /// </summary>
        public Dictionary<DBType, string> ComboItemSource
        {
            get
            {
                return new Dictionary<DBType, string>
                {
                    { DBType.MicrosoftSQLServer, DBTypes.GetName(DBType.MicrosoftSQLServer) },
                    { DBType.MicrosoftOleDb, DBTypes.GetName(DBType.MicrosoftOleDb) },
                    { DBType.PostgreSQL, DBTypes.GetName(DBType.PostgreSQL) },
                    { DBType.MySQL, DBTypes.GetName(DBType.MySQL) }
                };
            }
        }

        /// <summary>
        /// 選択中のDB種別
        /// </summary>
        public DBType? SelectedDBType
        {
            get { return m_selectedDBType; }
            set
            {
                if (value != m_selectedDBType)
                {
                    m_selectedDBType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DBType? m_selectedDBType;

        /// <summary>
        /// 選択中のDB種別名
        /// </summary>
        public string SelectedDBTypeName
        {
            get { return DBTypes.GetName(SelectedDBType); }
        }

        /// <summary>
        /// DB種別コンボの有効無効
        /// </summary>
        public bool IsComboEnabled
        {
            get { return m_isComboEnabled; }
            set
            {
                if (value != m_isComboEnabled)
                {
                    m_isComboEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isComboEnabled;

        /// <summary>
        /// DB種別変更時コマンド
        /// </summary>
        public ICommand ComboChangeCommand
        {
            get
            {
                if (m_comboChangeCommand == null)
                {
                    m_comboChangeCommand = new RelayCommand(() =>
                    {
                        SwitchUserControlByDBType();
                    });
                }
                return m_comboChangeCommand;
            }
        }
        private RelayCommand? m_comboChangeCommand;

        /// <summary>
        /// 設定ファイルパス文字列
        /// </summary>
        public string FilePathText
        {
            get { return m_filePathText; }
            set
            {
                if (value != m_filePathText)
                {
                    m_filePathText = @value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_filePathText = string.Empty;

        /// <summary>
        /// 設定ファイル選択コマンド
        /// </summary>
        public ICommand OpenFileCommand
        {
            get
            {
                if (m_openFileCommand == null)
                {
                    m_openFileCommand = new RelayCommand(() =>
                    {
                        var ofd = new CommonOpenFileDialog()
                        {
                            Title = "設定ファイル選択",
                            Multiselect = false,
                        };
                        var filter = new CommonFileDialogFilter("YERSファイル", "*.yers");
                        ofd.Filters.Add(filter);

                        if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            FilePathText = ofd.FileName;
                        }
                    });
                }
                return m_openFileCommand;
            }
        }
        private RelayCommand? m_openFileCommand;

        /// <summary>
        /// 設定ファイルパスを削除コマンド
        /// </summary>
        public ICommand DeleteFilePathCommand
        {
            get
            {
                if (m_deleteFilePathCommand == null)
                {
                    m_deleteFilePathCommand = new RelayCommand(() =>
                    {
                        FilePathText = string.Empty;
                    });
                }
                return m_deleteFilePathCommand;
            }
        }
        private RelayCommand? m_deleteFilePathCommand;

        /// <summary>
        /// 表示するDB種別のViewModel
        /// </summary>
        public DBSettingViewModel? ContentViewModel
        {
            get { return m_contentViewModel; }
            set
            {
                if (value != m_contentViewModel)
                {
                    m_contentViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DBSettingViewModel? m_contentViewModel;
        private DBSettingMSSQLViewModel? m_msSqlViewModel;
        private DBSettingMSOLEViewModel? m_msOleViewModel;
        private DBSettingPostgreSQLViewModel? m_postgreSqlViewModel;
        private DBSettingMySQLViewModel? m_mySqlViewModel;


        /// <summary>
        /// OKボタン処理[データベース接続設定を決定]
        /// </summary>
        protected override bool OKCommandMethod()
        {
            Action<DBType> errorMessage = (dbType) =>
            {
                MessageBox.Show(
                    "データベース名を入力してください。",
                    DBTypes.GetName(dbType),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            Func<DBSettingViewModel?, bool> isNullInput = (viewmodel) =>
            {
                if (viewmodel == null) return true;
                var bCheck = string.IsNullOrWhiteSpace(viewmodel.ServerText);
                bCheck |= string.IsNullOrWhiteSpace(viewmodel.ServerText);
                bCheck |= string.IsNullOrWhiteSpace(viewmodel.DatabaseText);
                if (viewmodel.IsIntegrate == true)
                {
                    bCheck |= string.IsNullOrWhiteSpace(viewmodel.UserText);
                    bCheck |= string.IsNullOrWhiteSpace(viewmodel.PasswordText);
                }
                return bCheck;
            };

            switch (m_selectedDBType)
            {
                case DBType.MicrosoftSQLServer:
                    if (isNullInput(m_msSqlViewModel))
                    {
                        errorMessage(DBType.MicrosoftSQLServer);
                        return false;
                    }
                    m_targetDBResource = m_msSqlViewModel?.GetDataSource();
                    break;
                case DBType.MicrosoftOleDb:
                    if (isNullInput(m_msOleViewModel))
                    {
                        errorMessage(DBType.MicrosoftOleDb);
                        return false;
                    }
                    m_targetDBResource = m_msOleViewModel?.GetDataSource();
                    break;
                case DBType.PostgreSQL:
                    if (isNullInput(m_postgreSqlViewModel))
                    {
                        errorMessage(DBType.PostgreSQL);
                        return false;
                    }
                    m_targetDBResource = m_postgreSqlViewModel?.GetDataSource();
                    break;
                case DBType.MySQL:
                    if (isNullInput(m_mySqlViewModel))
                    {
                        errorMessage(DBType.MySQL);
                        return false;
                    }
                    m_targetDBResource = m_mySqlViewModel?.GetDataSource();
                    break;
                default:
                    NoSelectMessage();
                    break;
            }

            if (m_targetDBResource == null) return false;

            m_targetDBResource.SettingFilePath = FilePathText;
            return true;
        }

        /// <summary>
        /// DB種別ごとに設定画面を切替します。
        /// </summary>
        private void SwitchUserControlByDBType()
        {
            switch (m_selectedDBType)
            {
                case DBType.MicrosoftSQLServer:

                    if (m_msSqlViewModel == null)
                    {
                        m_msSqlViewModel = new DBSettingMSSQLViewModel(m_targetDBResource);
                    }
                    ContentViewModel = m_msSqlViewModel;
                    return;

                case DBType.MicrosoftOleDb:

                    if (m_msOleViewModel == null)
                    {
                        m_msOleViewModel = new DBSettingMSOLEViewModel(m_targetDBResource);
                    }
                    ContentViewModel = m_msOleViewModel;
                    return;

                case DBType.PostgreSQL:

                    if (m_postgreSqlViewModel == null)
                    {
                        m_postgreSqlViewModel = new DBSettingPostgreSQLViewModel(m_targetDBResource);
                    }
                    ContentViewModel = m_postgreSqlViewModel;
                    return;

                case DBType.MySQL:

                    if (m_mySqlViewModel == null)
                    {
                        m_mySqlViewModel = new DBSettingMySQLViewModel(m_targetDBResource);
                    }
                    ContentViewModel = m_mySqlViewModel;
                    return;

                default:
                    NoSelectMessage();
                    return;
            }
        }

        /// <summary>
        /// DB種別が未選択時のエラーメッセージ
        /// </summary>
        /// <returns>MessageBoxResult</returns>
        private MessageBoxResult NoSelectMessage()
        {
            return
                MessageBox.Show(
                    "設定が選択されていません。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }
}
