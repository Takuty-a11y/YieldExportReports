using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using YieldExportReports.Commands;
using YieldExportReports.Dock;
using YieldExportReports.Report.ExportReports;
using YieldExportReports.Report.ReportSettings;
using YieldExportReports.Utils;

namespace YieldExportReports.ViewModels.Report
{
    public class ReportTemplateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ReportTemplateViewModel()
        {
            m_openUrlCommand = new OpenUrlCommand();
        }

        public ReportTemplateSetting TemplateSetting { get; set; } = new();

        /// <summary>
        /// テンプレートファイルパス文字列
        /// </summary>
        public string FilePathText
        {
            get { return TemplateSetting.TemplatePath; }
            set
            {
                if (value != TemplateSetting.TemplatePath)
                {
                    TemplateSetting.TemplatePath = @value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// テンプレートファイル選択コマンド
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
                            Title = "テンプレートファイル選択",
                            Multiselect = false,
                        };
                        var filter = new CommonFileDialogFilter("Excelファイル", "*.xlsx;*.xlsm;*.xls");
                        ofd.Filters.Add(filter);

                        if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            FilePathText = ofd.FileName;
                            GetSheets();
                        }
                    });
                }
                return m_openFileCommand;
            }
        }
        private RelayCommand? m_openFileCommand;

        /// <summary>
        /// テンプレートシートの選択肢
        /// </summary>
        public Dictionary<int, string> SheetItemSource
        {
            get { return m_sheetItemSource; }
            set
            {
                if (value != m_sheetItemSource)
                {
                    m_sheetItemSource = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Dictionary<int, string> m_sheetItemSource = new();

        /// <summary>
        /// 選択中のテンプレートシートKey
        /// </summary>
        public int SelectedSheetKey
        {
            get { return TemplateSetting.TemplateSheetIndex; }
            set
            {
                if (value != TemplateSetting.TemplateSheetIndex)
                {
                    TemplateSetting.TemplateSheetIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中のテンプレートシートValue
        /// </summary>
        public string SelectedSheetValue
        {
            get
            {
                if (!m_sheetItemSource.ContainsKey(SelectedSheetKey))
                { return string.Empty; }
                else
                { return m_sheetItemSource[SelectedSheetKey]; }
            }
        }

        /// <summary>
        /// シート取得コマンド
        /// </summary>
        public ICommand GetSheetCommand
        {
            get
            {
                if (m_getSheetCommand == null)
                {
                    m_getSheetCommand = new RelayCommand(() =>
                    {
                        GetSheets();
                    });
                }
                return m_getSheetCommand;
            }
        }
        private RelayCommand? m_getSheetCommand;

        /// <summary>
        /// テンプレートファイルの全シートを取得します
        /// </summary>
        internal void GetSheets()
        {
            if (string.IsNullOrWhiteSpace(FilePathText))
            { return; }

            try
            {
                SheetItemSource =
                    ExportReportFactory.GetTemplateSheets(FilePathText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "シート読込エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// テンプレートファイル起動コマンド
        /// </summary>
        public ICommand OpenUrlCommand => m_openUrlCommand;
        private readonly OpenUrlCommand m_openUrlCommand;

        /// <summary>
        /// 一時ファイルパス文字列
        /// </summary>
        public string TmpFilePathText
        {
            get { return TemplateSetting.TemporaryFilePath; }
            set
            {
                if (value != TemplateSetting.TemporaryFilePath)
                {
                    TemplateSetting.TemporaryFilePath = @value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 一時ファイル保存先選択コマンド
        /// </summary>
        public ICommand OpenTmpFileCommand
        {
            get
            {
                if (m_openTmpFileCommand == null)
                {
                    m_openTmpFileCommand = new RelayCommand(() =>
                    {
                        var ofd = new CommonOpenFileDialog()
                        {
                            Title = "一時ファイル保存先選択",
                            IsFolderPicker = true,
                            Multiselect = false,
                        };
                        if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            TmpFilePathText = ofd.FileName;
                        }
                    });
                }
                return m_openTmpFileCommand;
            }
        }
        private RelayCommand? m_openTmpFileCommand;

        /// <summary>
        /// 一時シート名
        /// </summary>
        public string TmpSheetName
        {
            get { return TemplateSetting.TemporarySheetName; }
            set
            {
                if (value != TemplateSetting.TemporarySheetName)
                {
                    TemplateSetting.TemporarySheetName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 各コントロールへ変更を通知します
        /// </summary>
        public void NotifyProperty()
        {
            var properties =
                AppHelper.GetMyProperties<ReportTemplateViewModel>();
            foreach (var name in from info in properties
                                 select info.Name)
            {
                NotifyPropertyChanged(name);
            }
            GetSheets();
        }
    }
}
