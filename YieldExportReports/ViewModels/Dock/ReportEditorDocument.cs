using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using YieldExportReports.Dock;
using YieldExportReports.Report.ExportReports;
using YieldExportReports.Report.ReportSettings;
using YieldExportReports.Utils;
using YieldExportReports.ViewModels.Report;

namespace YieldExportReports.ViewModels.Dock
{
    public sealed class ReportEditorDocument : ToolContent
    {
        public ReportEditorDocument() : base(string.Empty)
        {
        }
        public ReportEditorDocument(string contentId, string? title = null) : base(contentId, title)
        {
            IsDocument = true;
            SetSettingValue();
        }

        /// <summary>
        /// 出力設定データ
        /// </summary>
        public ExportReport ExportSetting { get; set; } = new ();

        #region *設定値取得

        /// <summary>
        /// 設定値を取得します
        /// </summary>
        /// <param name="exportReport">設定値</param>
        public void GetSettingValue(ExportReport exportReport)
        {
            ExportSetting = exportReport;
            SetSettingValue();
        }

        /// <summary>
        /// 設定値を下位ViewModelへセットします
        /// </summary>
        private void SetSettingValue()
        {
            GeneralViewModel.GeneralSetting = ExportSetting.GeneralSetting;
            TemplateViewModel.TemplateSetting = ExportSetting.TemplateSetting;
            GeneralViewModel.NotifyProperty();
            TemplateViewModel.NotifyProperty();
            NotifyProperty();
        }

        #endregion

        #region *全般設定データ

        /// <summary>
        /// 全般の設定データ
        /// </summary>
        public ReportGeneralViewModel GeneralViewModel
        {
            get { return m_generalViewModel; }
            set
            {
                if (value != m_generalViewModel)
                {
                    m_generalViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportGeneralViewModel m_generalViewModel = new();

        /// <summary>
        /// 全般タブが選択中であるか
        /// </summary>
        public bool IsSelectGeneral
        {
            get { return m_isSelectGeneral; }
            set
            {
                if (value != m_isSelectGeneral)
                {
                    m_isSelectGeneral = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isSelectGeneral = true;

        #endregion

        #region *テンプレート設定データ

        /// <summary>
        /// テンプレートの設定データ
        /// </summary>
        public ReportTemplateViewModel TemplateViewModel
        {
            get { return m_templateViewModel; }
            set
            {
                if (value != m_templateViewModel)
                {
                    m_templateViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportTemplateViewModel m_templateViewModel = new();

        /// <summary>
        /// テンプレートタブが選択中であるか
        /// </summary>
        public bool IsSelectTemplate
        {
            get { return m_isSelectTemplate; }
            set
            {
                if (value != m_isSelectTemplate)
                {
                    m_isSelectTemplate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isSelectTemplate;

        #endregion

        #region *分割設定データ

        /// <summary>
        /// ファイル分割出力の設定データ
        /// </summary>
        public List<ReportSetting> SplitFileDataContext
        {
            get { return ExportSetting.FileSplitData; }
            set
            {
                if (value != ExportSetting.FileSplitData)
                {
                    ExportSetting.FileSplitData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中のファイル分割設定
        /// </summary>
        public ReportSetting? SelectedSplitFile
        {
            get { return m_selectedSplitFile; }
            set
            {
                if (value != m_selectedSplitFile)
                {
                    m_selectedSplitFile = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportSetting? m_selectedSplitFile;

        /// <summary>
        /// ファイル分割設定の削除コマンド
        /// </summary>
        public ICommand DeleteSplitFile
        {
            get
            {
                if (m_deleteSplitFile == null)
                {
                    m_deleteSplitFile = new RelayCommand(() =>
                    {
                        if (m_selectedSplitFile != null)
                        {
                            SplitFileDataContext =
                                DeleteDataList(SplitFileDataContext, m_selectedSplitFile);
                        }
                    });
                }
                return m_deleteSplitFile;
            }
        }
        private RelayCommand? m_deleteSplitFile;

        /// <summary>
        /// シート分割出力の設定データ
        /// </summary>
        public List<ReportSetting> SplitSheetDataContext
        {
            get { return ExportSetting.SheetSplitData; }
            set
            {
                if (value != ExportSetting.SheetSplitData)
                {
                    ExportSetting.SheetSplitData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中のシート分割設定
        /// </summary>
        public ReportSetting? SelectedSplitSheet
        {
            get { return m_selectedSplitSheet; }
            set
            {
                if (value != m_selectedSplitSheet)
                {
                    m_selectedSplitSheet = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportSetting? m_selectedSplitSheet;

        /// <summary>
        /// シート分割設定の削除コマンド
        /// </summary>
        public ICommand DeleteSplitSheet
        {
            get
            {
                if (m_deleteSplitSheet == null)
                {
                    m_deleteSplitSheet = new RelayCommand(() =>
                    {
                        if (m_selectedSplitSheet != null)
                        {
                            SplitSheetDataContext =
                                DeleteDataList(SplitSheetDataContext, m_selectedSplitSheet);
                        }
                    });
                }
                return m_deleteSplitSheet;
            }
        }
        private RelayCommand? m_deleteSplitSheet;

        /// <summary>
        /// 分割設定タブが選択中であるか
        /// </summary>
        public bool IsSelectSplit
        {
            get { return m_isSelectSplit; }
            set
            {
                if (value != m_isSelectSplit)
                {
                    m_isSelectSplit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isSelectSplit;

        #endregion

        #region *出力設定データ

        /// <summary>
        /// セル出力の設定データ
        /// </summary>
        public List<ReportSetting> CellDataContext
        {
            get { return ExportSetting.CellOutputData; }
            set
            {
                if (value != ExportSetting.CellOutputData)
                {
                    ExportSetting.CellOutputData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中のセル出力設定
        /// </summary>
        public ReportSetting? SelectedCell
        {
            get { return m_selectedCell; }
            set
            {
                if (value != m_selectedCell)
                {
                    m_selectedCell = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportSetting? m_selectedCell;

        /// <summary>
        /// セル出力設定の削除コマンド
        /// </summary>
        public ICommand DeleteCell
        {
            get
            {
                if (m_deleteCell == null)
                {
                    m_deleteCell = new RelayCommand(() =>
                    {
                        if (m_selectedCell != null)
                        {
                            CellDataContext = DeleteDataList(CellDataContext, m_selectedCell);
                        }
                    });
                }
                return m_deleteCell;
            }
        }
        private RelayCommand? m_deleteCell;

        /// <summary>
        /// 連続s出力の開始行
        /// </summary>
        public int RepeatStartIndex
        {
            get { return ExportSetting.RepeatStartRow; }
            set
            {
                if (value != ExportSetting.RepeatStartRow)
                {
                    ExportSetting.RepeatStartRow = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 連続s出力の終了行
        /// </summary>
        public int RepeatEndIndex
        {
            get { return ExportSetting.RepeatEndRow; }
            set
            {
                if (value != ExportSetting.RepeatEndRow)
                {
                    ExportSetting.RepeatEndRow = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 連続出力の設定データ
        /// </summary>
        public List<ReportSetting> RepeatDataContext
        {
            get { return ExportSetting.RepeatOutputData; }
            set
            {
                if (value != ExportSetting.RepeatOutputData)
                {
                    ExportSetting.RepeatOutputData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中の連続出力設定
        /// </summary>
        public ReportSetting? SelectedRepeat
        {
            get { return m_selectedRepeat; }
            set
            {
                if (value != m_selectedRepeat)
                {
                    m_selectedRepeat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportSetting? m_selectedRepeat;

        /// <summary>
        /// 連続出力設定の削除コマンド
        /// </summary>
        public ICommand DeleteRepeat
        {
            get
            {
                if (m_deleteRepeat == null)
                {
                    m_deleteRepeat = new RelayCommand(() =>
                    {
                        if (m_selectedRepeat != null)
                        {
                            RepeatDataContext = DeleteDataList(RepeatDataContext, m_selectedRepeat);
                        }
                    });
                }
                return m_deleteRepeat;
            }
        }
        private RelayCommand? m_deleteRepeat;

        /// <summary>
        /// 出力設定タブが選択中であるか
        /// </summary>
        public bool IsSelectCell
        {
            get { return m_isSelectCell; }
            set
            {
                if (value != m_isSelectCell)
                {
                    m_isSelectCell = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_isSelectCell;

        #endregion

        #region *上下グリッド判別

        /// <summary>
        /// 選択中タブのデータ（上）
        /// </summary>
        public List<ReportSetting>? SelectedData_Top
        {
            get
            {
                if (m_isSelectSplit)
                { return SplitFileDataContext; }
                else if (m_isSelectCell)
                { return CellDataContext; }
                else
                { return null; }
            }
            set
            {
                if (value != null)
                {
                    if (m_isSelectSplit)
                    { SplitFileDataContext = value; }
                    else if (m_isSelectCell)
                    { CellDataContext = value; }
                }
            }
        }

        /// <summary>
        /// 選択中タブのデータ（下）
        /// </summary>
        public List<ReportSetting>? SelectedData_Under
        {
            get
            {
                if (m_isSelectSplit)
                { return SplitSheetDataContext; }
                else if (m_isSelectCell)
                { return RepeatDataContext; }
                else
                { return null; }
            }
            set
            {
                if (value != null)
                {
                    if (m_isSelectSplit)
                    { SplitSheetDataContext = value; }
                    else if (m_isSelectCell)
                    { RepeatDataContext = value; }
                }
            }
        }

        #endregion

        #region *設定追加/削除/更新

        /// <summary>
        /// 設定値を上部の設定へ追加します
        /// </summary>
        /// <param name="newItem">追加対象データ</param>
        public void AddData_Top(ReportSetting newItem)
        { SelectedData_Top = AddDataList(SelectedData_Top, newItem); }

        /// <summary>
        /// 設定値を下部の設定へ追加します
        /// </summary>
        /// <param name="newItem">追加対象データ</param>
        public void AddData_Under(ReportSetting newItem)
        { SelectedData_Under = AddDataList(SelectedData_Under, newItem); }

        /// <summary>
        /// 設定を追加し、そのデータを返却します
        /// </summary>
        /// <param name="settings">上下判定</param>
        /// <param name="newItem">追加対象データ</param>
        /// <returns>追加後リスト</returns>
        private List<ReportSetting>? AddDataList
            (List<ReportSetting>? settings, ReportSetting newItem)
        {
            try
            {
                var newItemField = newItem.Field ?? throw new ArgumentNullException("追加フィールド");
                var oldList = settings ?? throw new ArgumentNullException("追加先データ");
                var lstData = new List<ReportSetting>(settings);
                var sameItem = from r in lstData
                               where r.Field.ID == newItemField.ID
                               select r;

                if (!sameItem.Any()) { lstData.Add(newItem); }

                return lstData;
            }
            catch
            {
                return settings;
            }
        }

        /// <summary>
        /// 設定を削除し、そのデータを返却します
        /// </summary>
        /// <param name="settings">削除対象リスト</param>
        /// <param name="deleteItem">削除対象データ</param>
        /// <returns>削除後リスト</returns>
        private List<ReportSetting> DeleteDataList
            (List<ReportSetting> settings, ReportSetting deleteItem)
        {
            try
            {
                var lstData = new List<ReportSetting>(settings);
                lstData.Remove(deleteItem);
                return lstData;
            }
            catch
            {
                return settings;
            }
        }

        /// <summary>
        /// 各コントロールへ変更を通知します
        /// </summary>
        public void NotifyProperty()
        {
            var properties =
                AppHelper.GetMyProperties<ReportEditorDocument>();
            foreach (var name in from info in properties
                                 select info.Name)
            {
                NotifyPropertyChanged(name);
            }
        }

        #endregion

    }
}
