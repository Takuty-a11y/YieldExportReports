using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Commands;
using YieldExportReports.Database.DBConnects;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBObjects;
using YieldExportReports.Database.DBResources;
using YieldExportReports.Dock;
using YieldExportReports.Report.ExportReports;
using YieldExportReports.Report.ReportExceptions;
using YieldExportReports.Report.ReportFields;
using YieldExportReports.Report.ReportLibraries;
using YieldExportReports.Report.ReportSettings;
using YieldExportReports.Utils;
using YieldExportReports.ViewModels.Dock;
using YieldExportReports.ViewModels.Login;
using YieldExportReports.Views.Login;
using YieldExportReports.Views.Main;

namespace YieldExportReports.ViewModels.Main
{
    public class MainWindowViewModel : ViewModelBase
    {
        // 定数
        public const string QueryEditorDocumentID = "QueryEditor";
        public const string ReportEditorDocumentID = "ReportEditor";
        public const string TreeViewContentID = "TreeView";
        public const string ReportToolContentID = "ReportTool";
        public const string GridViewContentID = "GridView";

        // 変数
        private DBConnect m_targetDBConnect = new();
        private readonly ShowWindowService<LoginWindow, LoginWindowViewModel>? m_loginWindowService;
        private CancellationTokenSource? m_takenCancel = null;

        // コマンド
        public ICommand LoadedCommand => m_loadedCommand;
        private readonly WindowLoadedCommand m_loadedCommand;
        public ICommand ClosedCommand => m_closedCommand;
        private readonly WindowClosedCommand m_closedCommand;
        public ICommand MenuCommand => m_menuCommand;
        private readonly WindowMenuCommand m_menuCommand;
        public ICommand OpenUrlCommand => m_openUrlCommand;
        private readonly OpenUrlCommand m_openUrlCommand;
        public ICommand RenderedCommand
        {
            get
            {
                if (m_renderedCommand == null)
                {
                    m_renderedCommand = new RelayCommand(() =>
                    {
                        ShowLogin();
                    });
                }
                return m_renderedCommand;
            }
        }
        private RelayCommand? m_renderedCommand;

        // コンストラクタ
        public MainWindowViewModel()
        {
            m_loadedCommand = new WindowLoadedCommand(this);
            m_closedCommand = new WindowClosedCommand(this);
            m_menuCommand = new WindowMenuCommand(this);
            m_openUrlCommand = new OpenUrlCommand();
        }
        public MainWindowViewModel(
            ShowWindowService<LoginWindow, LoginWindowViewModel> loginWindowService) : this()
        {
            m_loginWindowService = loginWindowService;
        }

        #region *[プロパティ]設定ファイル

        /// <summary>
        /// 設定ファイルパス
        /// </summary>
        public string SettingFilePath { get; set; } = string.Empty;

        #endregion

        #region *[プロパティ]ドック

        /// <summary>
        /// [DockContent]クエリエディタ
        /// </summary>
        public QueryEditorDocument QueryEditorViewModel
        {
            get { return (QueryEditorDocument)GetContentByContentId(QueryEditorDocumentID); }
        }
        /// <summary>
        /// [DockContent]レポートエディタ
        /// </summary>
        public ReportEditorDocument ReportEditorViewModel
        {
            get { return (ReportEditorDocument)GetContentByContentId(ReportEditorDocumentID); }
        }
        /// <summary>
        /// [DockContent]データベースツリー
        /// </summary>
        public DBTreeViewTool DBTreeViewModel
        {
            get { return (DBTreeViewTool)GetContentByContentId(TreeViewContentID); }
        }
        /// <summary>
        /// [DockContent]レポートツール
        /// </summary>
        public ReportFieldViewTool ReportFieldViewModel
        {
            get { return (ReportFieldViewTool)GetContentByContentId(ReportToolContentID); }
        }
        /// <summary>
        /// [DockContent]データビュー
        /// </summary>
        public DataGridViewTool DataGridViewModel
        {
            get { return (DataGridViewTool)GetContentByContentId(GridViewContentID); }
        }

        /// <summary>
        /// DockContentを生成します
        /// </summary>
        protected override void InitializeTools()
        {
            Tools.Clear();
            Tools.Add(new QueryEditorDocument(QueryEditorDocumentID, "クエリエディタ"));
            Tools.Add(new ReportEditorDocument(ReportEditorDocumentID, "レポートエディタ"));
            Tools.Add(new DBTreeViewTool(TreeViewContentID, "データベースツリー"));
            Tools.Add(new ReportFieldViewTool(ReportToolContentID, "レポートツール"));
            Tools.Add(new DataGridViewTool(GridViewContentID, "データビュー"));

            InitializeToolsEvent();
        }

        /// <summary>
        /// DockContentのイベントを設定します
        /// </summary>
        private void InitializeToolsEvent()
        {
            QueryEditorViewModel.GetDataFromQuery = (sQuery) => GetDataFromQuery(sQuery);
            QueryEditorViewModel.GetDataFromQueryCancel = () => GetQueryDataCancel();
            DBTreeViewModel.DBReConnect = () => ChangeDBConnection();
            DBTreeViewModel.GetLatestDBInfo = () => ExecDBConnection(true);
            DBTreeViewModel.SetSelectQuery = (sQuery) => GetQueryDataFromTable(sQuery);
            ReportFieldViewModel.RunExportReport = () => ExecuteExportReport();
            ReportFieldViewModel.AddTopNewSetting = (newItem) => AddTopSetting(newItem);
            ReportFieldViewModel.AddUnderNewSetting = (newItem) => AddUnderSetting(newItem);
            ReportFieldViewModel.GetLatestField = () => SetReportFieldFromGrid();
            ReportFieldViewModel.SaveSetting = () => SaveFile();
        }

        #endregion

        #region *[プロパティ]フッター

        /// <summary>
        /// [フッター]アプリケーションバージョン
        /// </summary>
        public string AppVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? string.Empty;
            }
        }
        /// <summary>
        /// [フッター]接続中のデータベース種別
        /// </summary>
        public string TargetDBType
        {
            get
            {
                if (m_targetDBConnect != null)
                {
                    var sDBType = DBTypes.GetName(m_targetDBConnect.DBType);
                    if (!string.IsNullOrEmpty(sDBType)) return sDBType;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// [フッター]接続中の接続先名
        /// </summary>
        public string TargetDataSource
        {
            get
            {
                if (m_targetDBConnect != null)
                {
                    return m_targetDBConnect.ConnectionName;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// [フッター]接続中のデータベース名
        /// </summary>
        public string TargetDBName
        {
            get
            {
                if (m_targetDBConnect != null)
                {
                    return m_targetDBConnect.DefaultDB;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// [フッター]設定ファイル名
        /// </summary>
        public string TargetSettingFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SettingFilePath))
                {
                    return Path.GetFileName(SettingFilePath);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// DB情報のフッター表示値を更新します
        /// </summary>
        private void FooterValueChange()
        {
            NotifyPropertyChanged(nameof(TargetDBType));
            NotifyPropertyChanged(nameof(TargetDataSource));
            NotifyPropertyChanged(nameof(TargetDBName));
            NotifyPropertyChanged(nameof(TargetSettingFile));
        }

        #endregion

        #region *[プロパティ]メニュー

        /// <summary>
        /// ツールからデータを取得します
        /// </summary>
        public async void GetDataFromTool()
            => await GetQueryData(QueryEditorViewModel.EditorDocument.Text);

        /// <summary>
        /// ツールからレポートを出力します
        /// </summary>
        public void ExportReportFromTool()
            => ExecuteExportReport();

        /// <summary>
        /// サードパーティ情報画面を表示します
        /// </summary>
        public void ShowThirdPartyInfo()
        {
            var dlg = new ThirdPartyInfo
            {
                Owner = m_loginWindowService?.Owner
            };
            dlg.ShowDialog();
        }

        /// <summary>
        /// バージョン情報画面を表示します
        /// </summary>
        public void ShowVersionInfo()
        {
            var dlg = new ProductInfo
            {
                Owner = m_loginWindowService?.Owner
            };
            dlg.ShowDialog();
        }

        #endregion

        #region *ログイン処理

        /// <summary>
        /// ログイン処理を行います
        /// </summary>
        private void ShowLogin()
        {
            try
            {
                var bDialogResult =
                    m_loginWindowService?.ShowDialog(new LoginWindowViewModel());

                if (bDialogResult != true)
                {
                    if (MessageBox.Show(
                            "アプリケーションを終了しますか？"
                            , nameof(YieldExportReports)
                            , MessageBoxButton.OKCancel
                            , MessageBoxImage.Question)
                        == MessageBoxResult.OK)
                    { Application.Current.Shutdown(); }
                    else
                    { ShowLogin(); }
                }

                ExecDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// [非同期]接続先のDB情報を取得します
        /// </summary>
        private async void ExecDBConnection(bool isOnlyDB = false)
        {
            var dBObjectCollection = new DBObjectCollection();

            var dlg = new LoadingDialog
            {
                Header = "データベース接続中",
                Owner = m_loginWindowService?.Owner
            };
            dlg.Show();

            try
            {
                await Task.Run(() =>
                {
                    var dbResourceCollection = DBResourceCollectionFactory.GetDBResource();
                    foreach (var dBResource in from r in dbResourceCollection
                                               where r.IsConnected
                                               select r)
                    {
                        m_targetDBConnect = new DBConnect()
                        {
                            ConnectionName = dBResource.Name,
                            DefaultDB = dBResource.DefaultDB,
                            InstanceName = dBResource.InstanceName,
                            DBType = dBResource.DBType,
                            PersistSecurityInfo = dBResource.PersistSecurityInfo,
                            UserName = dBResource.UserName,
                            Password = dBResource.Password,
                            TimeOut = dBResource.TimeOut,
                            IntegratedSecurity = dBResource.IntegratedSecurity,
                        };

                        SettingFilePath = dBResource.SettingFilePath;
                        FooterValueChange();

                        using (m_targetDBConnect.Connection = DBConnectFactory.GetConnection(dBResource.DBType, dBResource.ConnectionString))
                        {
                            m_targetDBConnect.Connection.Open();
                            m_targetDBConnect.IsConnected = true;
                            var sqlOperator = DBConnectFactory.CreateOperator(dBResource.DBType);
                            dBObjectCollection = sqlOperator.GetDBObjectsToList(m_targetDBConnect.Connection);
                        }
                        break;
                    }
                    DBTreeViewModel.DbObjectCollection = dBObjectCollection;
                });
                if (!isOnlyDB) SetSettingValueToDocks();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dlg.Close();
            }
        }

        #endregion

        #region *[イベント]設定ファイル

        /// <summary>
        /// 設定値を取得し各Dockへ反映します
        /// </summary>
        private void SetSettingValueToDocks()
        {
            try
            {
                var settingValue = ExportReportFactory.Deserialize(SettingFilePath)
                    ?? throw new ArgumentNullException("ExportReport");

                ReportEditorViewModel.GetSettingValue(settingValue);

                QueryEditorViewModel.EditorDocument.Text = settingValue.SQLText;

                var fields = settingValue.ResultFields
                    ?? throw new ArgumentNullException("ResultFields");

                var dt = new DataTable();
                foreach (var name in from field in fields
                                     orderby field.Index
                                     select field.Name)
                {
                    dt.Columns.Add(new DataColumn(name));
                }
                DataGridViewModel.GridDataContext = dt;
                SetReportFieldFromGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "設定ファイル読込エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 設定ファイルを新規作成します
        /// </summary>
        public void CreateNewFile()
        {
            var dlgSave = new CommonSaveFileDialog()
            {
                Title = "保存先ファイル選択",
            };
            var filter = new CommonFileDialogFilter("YERSファイル", "*.yers");
            dlgSave.Filters.Add(filter);

            if (dlgSave.ShowDialog() == CommonFileDialogResult.Ok)
            {
                QueryEditorViewModel.EditorDocument.Text = string.Empty;
                DataGridViewModel.GridDataContext = null;
                ReportFieldViewModel.FieldDataContext = null;
                ReportEditorViewModel.GetSettingValue(new ExportReport());

                SettingFilePath = dlgSave.FileName;
                FooterValueChange();
            }
        }

        /// <summary>
        /// 設定値を保存します
        /// </summary>
        /// <param name="filePath">保存先</param>
        public void SaveFile(string filePath)
        {
            try
            {
                ExportReportFactory.Serialize
                       (GetSettingValueFromDocks(), filePath);

                SettingFilePath = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "設定ファイル保存失敗",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 設定値を保存します
        /// </summary>
        public void SaveFile()
        {
            if (string.IsNullOrWhiteSpace(SettingFilePath))
            { SaveFileAsNew(); }
            else
            { SaveFile(SettingFilePath); }
        }

        /// <summary>
        /// 設定値を新規保存します
        /// </summary>
        public void SaveFileAsNew()
        {
            var dlgSave = new CommonSaveFileDialog()
            {
                Title = "保存先ファイル選択",
            };
            var filter = new CommonFileDialogFilter("YERSファイル", "*.yers");
            dlgSave.Filters.Add(filter);

            if (dlgSave.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SaveFile(dlgSave.FileName);
            }
        }

        /// <summary>
        /// 設定値を初期状態へ戻します
        /// </summary>
        public void CloseFile()
        {
            QueryEditorViewModel.EditorDocument.Text = string.Empty;
            DBTreeViewModel.DbObjectCollection = null;
            DataGridViewModel.GridDataContext = null;
            ReportFieldViewModel.FieldDataContext = null;
            ReportEditorViewModel.GetSettingValue(new ExportReport());
            SettingFilePath = string.Empty;
            m_targetDBConnect = new DBConnect();
            FooterValueChange();
            ShowLogin();
        }

        /// <summary>
        /// 各Dockから設定値を取得します
        /// </summary>
        /// <returns>設定値</returns>
        private ExportReport GetSettingValueFromDocks()
        {
            var settingValue = ReportEditorViewModel.ExportSetting;
            settingValue.SQLText = QueryEditorViewModel.EditorDocument.Text;
            SetReportFieldFromGrid();
            settingValue.ResultFields = ReportFieldViewModel.FieldDataContext ?? new List<ReportField>();

            return settingValue;
        }

        #endregion

        #region *[イベント]クエリエディタ

        /// <summary>
        /// 現在のクエリからデータを取得します
        /// </summary>
        /// <param name="sQuery">クエリ</param>
        private async void GetDataFromQuery(string sQuery)
            => await GetQueryData(sQuery);

        /// <summary>
        /// [非同期]クエリから取得したデータを出力します
        /// </summary>
        private async Task GetQueryData(string sQuery)
        {
            QueryEditorViewModel.ExecEnabled = false;
            DataGridViewModel.GridDataContext = null;
            DataGridViewModel.GridVisible = Visibility.Collapsed;

            var sqlOperator = DBConnectFactory.CreateOperator(m_targetDBConnect.DBType);

            try
            {
                // Only SELECT
                if (string.IsNullOrEmpty(sQuery) ||
                    sQuery.ToUpper().Contains("DELETE", StringComparison.CurrentCulture) ||
                    sQuery.ToUpper().Contains("UPDATE", StringComparison.CurrentCulture) ||
                    sQuery.ToUpper().Contains("INSERT", StringComparison.CurrentCulture)) 
                    return;

                using (m_takenCancel = new CancellationTokenSource())
                {
                    var token = m_takenCancel.Token;

                    var dtResultData =
                        sqlOperator.GetQueryDataToDataTables(
                                        sQuery,
                                        m_targetDBConnect.ConnectionString,
                                        token);
                    var dtList = await dtResultData;

                    var setDataTable = DataGridViewModel.SetQuerytData(dtList, token);
                    await setDataTable;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SetReportFieldFromGrid();
                QueryEditorViewModel.ExecEnabled = true;
                DataGridViewModel.GridVisible = Visibility.Visible;
                m_takenCancel = null;
            }
        }

        /// <summary>
        /// [非同期]クエリからのデータ取得をキャンセルします
        /// </summary>
        private void GetQueryDataCancel()
        {
            if (m_takenCancel != null)
            {
                m_takenCancel.Cancel();
            }
        }

        #endregion

        #region *[イベント]データベースツリー

        /// <summary>
        /// 選択テーブルからデータを取得します
        /// </summary>
        /// <param name="sQuery">クエリ</param>
        private async void GetQueryDataFromTable(string sQuery)
        {
            ActiveDocument = QueryEditorViewModel;
            QueryEditorViewModel.EditorDocument.Text = sQuery;
            await GetQueryData(sQuery);
        }

        /// <summary>
        /// 接続中のDBを変更します
        /// </summary>
        internal void ChangeDBConnection()
        {
            Mouse.OverrideCursor = null;
            try
            {
                var dbResources = DBResourceCollectionFactory.GetDBResource();
                var bDialogResult =
                    m_loginWindowService?.ShowDialog(new LoginWindowViewModel());

                if (bDialogResult == true)
                { ExecDBConnection(); }
                else
                { DBResourceCollectionFactory.Serialize(dbResources); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region *[イベント]レポートツール

        /// <summary>
        /// データビューのフィールドをレポートエディタへ反映します
        /// </summary>
        private void SetReportFieldFromGrid()
        {
            var dtField = new ReportFieldCollection();
            if (DataGridViewModel.TryCreateExportData(out DataTable data))
            {
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    var col = data.Columns[i];
                    if (col == null) continue;

                    var item = new ReportField()
                    {
                        ID = Guid.NewGuid(),
                        Index = i,
                        TypeName = col.DataType.FullName ?? string.Empty,
                        Name = col.ColumnName,
                    };
                    dtField.Add(item);
                }
            }
            ReportFieldViewModel.FieldDataContext = dtField.ToList();
        }

        /// <summary>
        /// 設定一覧（上）へフィールドを追加します
        /// </summary>
        /// <param name="newSetting">追加フィールド</param>
        private void AddTopSetting(ReportSetting? newSetting)
        {
            if (newSetting == null) return;
            if (ActiveDocument.ContentId == ReportEditorDocumentID)
            {
                ReportEditorViewModel.AddData_Top(newSetting);
            }
        }

        /// <summary>
        /// 設定一覧（下）へフィールドを追加します
        /// </summary>
        /// <param name="newSetting">追加フィールド</param>
        private void AddUnderSetting(ReportSetting? newSetting)
        {
            if (newSetting == null) return;
            if (ActiveDocument.ContentId == ReportEditorDocumentID)
            {
                ReportEditorViewModel.AddData_Under(newSetting);
            }
        }

        /// <summary>
        /// [非同期]レポート出力します
        /// </summary>
        private async void ExecuteExportReport()
        {
            ReportFieldViewModel.ExecEnabled = false;
            try
            {
                var pathList = new List<string>();
                var settingValue = ReportEditorViewModel.ExportSetting;
                var ope = ExportReportFactory.CreateOperator(settingValue);

                //クエリからのデータを取得するか
                if (settingValue.GeneralSetting.IsQueryData)
                {
                    var sQuery = QueryEditorViewModel.EditorDocument.Text;
                    await GetQueryData(sQuery);
                }

                await Task.Run(() =>
                {
                    //出力
                    pathList = ope.RunExport(DataGridViewModel.GridDataContext);

                    //後処理
                    var openUrlCommand = new OpenUrlCommand();
                    switch (settingValue.GeneralSetting.PostProcess)
                    {
                        case PostProcessType.OpenFile:
                            foreach (var path in pathList)
                            {
                                if (openUrlCommand.CanExecute(path))
                                { openUrlCommand.Execute(path); }
                            }
                            break;

                        case PostProcessType.OpenFolder:
                            var folderPath = settingValue.GeneralSetting.ExportPath;
                            if (openUrlCommand.CanExecute(folderPath))
                            { openUrlCommand.Execute(folderPath); }
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                var sbMessage = new StringBuilder();
                sbMessage.AppendLine(ex.Message);
                if (ex is ReportException reportException)
                {
                    if (!string.IsNullOrWhiteSpace(reportException.Parameter))
                    { sbMessage.AppendLine(reportException.Parameter); }
                }

                MessageBox.Show(
                    sbMessage.ToString(),
                    "レポート出力エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            finally
            {
                ReportFieldViewModel.ExecEnabled = true;
            }
        }

        #endregion

    }
}
