using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Dock;
using YieldExportReports.Report.ReportFields;
using YieldExportReports.Report.ReportSettings;

namespace YieldExportReports.ViewModels.Dock
{
    public sealed class ReportFieldViewTool : ToolContent
    {
        public ReportFieldViewTool() : base(string.Empty)
        {
        }
        public ReportFieldViewTool(string contentId, string? title = null) : base(contentId, title)
        {
        }

        /// <summary>
        /// レポート生成ボタン有効無効
        /// </summary>
        public bool ExecEnabled
        {
            get { return m_execEnabled; }
            set
            {
                if (value != m_execEnabled)
                {
                    if (value) ExecText = "レポート出力";
                    else ExecText = "レポート出力中";
                    m_execEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool m_execEnabled = true;

        /// <summary>
        /// レポート生成ボタンテキスト
        /// </summary>
        public string ExecText
        {
            get { return m_execText; }
            set
            {
                if (value != m_execText)
                {
                    m_execText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string m_execText = "レポート出力";

        /// <summary>
        /// レポート生成コマンド
        /// </summary>
        public ICommand ExecCommand
        {
            get
            {
                if (m_execCommand == null)
                {
                    m_execCommand = new RelayCommand(() =>
                    {
                        RunExportReport?.Invoke();
                    });
                }
                return m_execCommand;
            }
        }
        private RelayCommand? m_execCommand;
        internal Action? RunExportReport { get; set; }

        /// <summary>
        /// 設定保存コマンド
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (m_saveCommand == null)
                {
                    m_saveCommand = new RelayCommand(() =>
                    {
                        SaveSetting?.Invoke();
                    });
                }
                return m_saveCommand;
            }
        }
        private RelayCommand? m_saveCommand;
        internal Action? SaveSetting { get; set; }


        /// <summary>
        /// フィールドデータ
        /// </summary>
        public List<ReportField>? FieldDataContext
        {
            get { return m_fieldDataContext; }
            set
            {
                if (value != m_fieldDataContext)
                {
                    m_fieldDataContext = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<ReportField>? m_fieldDataContext;

        /// <summary>
        /// 選択中のフィールド
        /// </summary>
        public ReportField? SelectedField
        {
            get { return m_selectedField; }
            set
            {
                if (value != m_selectedField)
                {
                    m_selectedField = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ReportField? m_selectedField;

        /// <summary>
        /// 新規設定値
        /// </summary>
        private ReportSetting? NewSettingItem
        {
            get
            {
                if (m_selectedField != null)
                {
                    return new ReportSetting
                    {
                        ID = Guid.NewGuid(),
                        Field = m_selectedField,
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// 設定一覧（上）へフィールドを追加コマンド
        /// </summary>
        public ICommand AddTopSettingCommand
        {
            get
            {
                if (m_addSettingCommand == null)
                {
                    m_addSettingCommand = new RelayCommand(() =>
                    {
                        AddTopNewSetting?.Invoke(NewSettingItem);
                    });
                }
                return m_addSettingCommand;
            }
        }
        private RelayCommand? m_addSettingCommand;
        internal Action<ReportSetting?>? AddTopNewSetting { get; set; }

        /// <summary>
        /// 設定一覧（下）へフィールドを追加コマンド
        /// </summary>
        public ICommand AddUnderSettingCommand
        {
            get
            {
                if (m_deleteSettingCommand == null)
                {
                    m_deleteSettingCommand = new RelayCommand(() =>
                    {
                        AddUnderNewSetting?.Invoke(NewSettingItem);
                    });
                }
                return m_deleteSettingCommand;
            }
        }
        private RelayCommand? m_deleteSettingCommand;
        internal Action<ReportSetting?>? AddUnderNewSetting { get; set; }

        /// <summary>
        /// 名称コピーコマンド
        /// </summary>
        public ICommand NameCopyCommand
        {
            get
            {
                if (m_nameCopyCommand == null)
                {
                    m_nameCopyCommand = new RelayCommand(() =>
                    {
                        Clipboard.SetText(SelectedField?.Name ?? string.Empty);
                    });
                }
                return m_nameCopyCommand;
            }
        }
        private RelayCommand? m_nameCopyCommand;

        /// <summary>
        /// データ再取得コマンド
        /// </summary>
        public ICommand LatestFieldCommand
        {
            get
            {
                if (m_latestFieldCommand == null)
                {
                    m_latestFieldCommand = new RelayCommand(() =>
                    {
                        GetLatestField?.Invoke();
                    });
                }
                return m_latestFieldCommand;
            }
        }
        private RelayCommand? m_latestFieldCommand;
        internal Action? GetLatestField { get; set; }
    }
}
