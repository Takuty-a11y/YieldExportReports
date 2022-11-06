using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Dock;
using YieldExportReports.Report.ExportFiles;

namespace YieldExportReports.ViewModels.Dock
{
    public sealed class DataGridViewTool : ToolContent
    {
        public const string HeaderRowID = "BFBB850F-6993-47DE-9D40-3757CB242747";

        public DataGridViewTool() : base(string.Empty) { }
        public DataGridViewTool(string contentId, string? title = null)
            : base(contentId, title)
        {
        }

        /// <summary>
        /// XML出力コマンド
        /// </summary>
        public ICommand XMLOutCommand
        {
            get
            {
                if (m_xmlOutCommand == null)
                {
                    m_xmlOutCommand = new RelayCommand(() =>
                    {
                        if (TryCreateExportData(out DataTable data))
                        {
                            ExportFileFactory.XML(data);
                        }
                    });
                }
                return m_xmlOutCommand;
            }
        }
        private RelayCommand? m_xmlOutCommand;

        /// <summary>
        /// JSON出力コマンド
        /// </summary>
        public ICommand JSONOutCommand
        {
            get
            {
                if (m_jsonOutCommand == null)
                {
                    m_jsonOutCommand = new RelayCommand(() =>
                    {
                        if (TryCreateExportData(out DataTable data))
                        {
                            ExportFileFactory.JSON(data);
                        }
                    });
                }
                return m_jsonOutCommand;
            }
        }
        private RelayCommand? m_jsonOutCommand;

        /// <summary>
        /// CSV出力コマンド
        /// </summary>
        public ICommand CSVOutCommand
        {
            get
            {
                if (m_csvOutCommand == null)
                {
                    m_csvOutCommand = new RelayCommand(() =>
                    {
                        if (TryCreateExportData(out DataTable data))
                        {
                            ExportFileFactory.CSV(data);
                        }
                    });
                }
                return m_csvOutCommand;
            }
        }
        private RelayCommand? m_csvOutCommand;

        /// <summary>
        /// Excel出力コマンド
        /// </summary>
        public ICommand ExcelOutCommand
        {
            get
            {
                if (m_excelOutCommand == null)
                {
                    m_excelOutCommand = new RelayCommand(() =>
                    {
                        if (TryCreateExportData(out DataTable data))
                        {
                            ExportFileFactory.EXCEL(data);
                        }
                    });
                }
                return m_excelOutCommand;
            }
        }
        private RelayCommand? m_excelOutCommand;

        /// <summary>
        /// グリッド表示データ
        /// </summary>
        public DataTable? GridDataContext
        {
            get { return m_gridDataContext; }
            set
            {
                if (value != m_gridDataContext)
                {
                    m_gridDataContext = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DataTable? m_gridDataContext;

        /// <summary>
        /// グリッド表示非表示
        /// </summary>
        public Visibility GridVisible
        {
            get { return m_gridVisible; }
            set
            {
                if (value != m_gridVisible)
                {
                    m_gridVisible = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(LoadingVisible));
                }
            }
        }
        private Visibility m_gridVisible = Visibility.Visible;

        /// <summary>
        /// ローディング表示非表示
        /// </summary>
        public Visibility LoadingVisible
        {
            get
            {
                return m_gridVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// [非同期]DBから取得したデータをグリッドへ表示
        /// </summary>
        /// <param name="dtResultData">グリッド</param>
        public async Task SetQuerytData
            (List<DataTable> dtResultData, CancellationToken token)
        {
            foreach (var dt in from r in dtResultData
                               where r.Rows.Count > 0
                               select r)
            {
                await Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                    { token.ThrowIfCancellationRequested(); }

                }, token);
                GridDataContext = dt;
            }
        }

        /// <summary>
        /// 出力用のデータを作成します
        /// </summary>
        /// <param name="data">出力用データ</param>
        /// <returns>作成結果</returns>
        public bool TryCreateExportData(out DataTable data)
        {
            data = new DataTable();
            try
            {
                data = m_gridDataContext?.Copy() ?? throw new Exception();
                data.Columns.Remove(HeaderRowID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
