using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using YieldExportReports.Dock;
using YieldExportReports.Report.ReportLibraries;
using YieldExportReports.Report.ReportSettings;
using YieldExportReports.Utils;

namespace YieldExportReports.ViewModels.Report
{
    public class ReportGeneralViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 全般設定の設定値
        /// </summary>
        public ReportGeneralSetting GeneralSetting { get; set; } = new();

        #region *出力パス

        /// <summary>
        /// レポート出力パス文字列
        /// </summary>
        public string FolderPathText
        {
            get { return GeneralSetting.ExportPath; }
            set
            {
                if (value != GeneralSetting.ExportPath)
                {
                    GeneralSetting.ExportPath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// レポート出力フォルダ選択
        /// </summary>
        public ICommand SelectFolderCommand
        {
            get
            {
                if (m_openFileCommand == null)
                {
                    m_openFileCommand = new RelayCommand(() =>
                    {
                        var fbd = new CommonOpenFileDialog()
                        {
                            Title = "レポート出力先選択",
                            IsFolderPicker = true,
                            Multiselect = false,
                        };
                        if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            FolderPathText = fbd.FileName;
                        }
                    });
                }
                return m_openFileCommand;
            }
        }
        private RelayCommand? m_openFileCommand;

        #endregion

        #region *ファイル設定

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileNameText
        {
            get { return GeneralSetting.FileName; }
            set
            {
                if (value != GeneralSetting.FileName)
                {
                    GeneralSetting.FileName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出力形式の選択肢
        /// </summary>
        public Dictionary<ReportFormatType, string> FormatItemSource
        {
            get
            {
                return new Dictionary<ReportFormatType, string>
                {
                    { ReportFormatType.Excel, ReportFormatTypes.GetName(ReportFormatType.Excel) },
                    { ReportFormatType.PDF, ReportFormatTypes.GetName(ReportFormatType.PDF) },
                };
            }
        }
        /// <summary>
        /// 選択中の出力形式Key
        /// </summary>
        public ReportFormatType SelectedFormatKey
        {
            get { return GeneralSetting.FormatType; }
            set
            {
                if (value != GeneralSetting.FormatType)
                {
                    GeneralSetting.FormatType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中の出力形式Value
        /// </summary>
        public string SelectedFormatValue
        {
            get { return FormatItemSource[SelectedFormatKey]; }
        }

        /// <summary>
        /// 分割指定の選択肢
        /// </summary>
        public Dictionary<ReportSplitType, string> SplitItemSource
        {
            get
            {
                return new Dictionary<ReportSplitType, string>
                {
                    { ReportSplitType.NoSplit, ReportSplitTypes.GetName(ReportSplitType.NoSplit) },
                    { ReportSplitType.File, ReportSplitTypes.GetName(ReportSplitType.File) },
                    { ReportSplitType.Sheet, ReportSplitTypes.GetName(ReportSplitType.Sheet) },
                    { ReportSplitType.FileSheet, ReportSplitTypes.GetName(ReportSplitType.FileSheet) },
                };
            }
        }
        /// <summary>
        /// 選択中の出力形式Key
        /// </summary>
        public ReportSplitType SelectedSplitKey
        {
            get { return GeneralSetting.SplitType; }
            set
            {
                if (value != GeneralSetting.SplitType)
                {
                    GeneralSetting.SplitType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中の出力形式Value
        /// </summary>
        public string SelectedSplitValue
        {
            get { return SplitItemSource[SelectedSplitKey]; }
        }

        #endregion

        #region *出力設定

        /// <summary>
        /// 出力後処理の選択肢
        /// </summary>
        public Dictionary<PostProcessType, string> PostProcessItemSource
        {
            get
            {
                return new Dictionary<PostProcessType, string>
                {
                    { PostProcessType.NoProcess, PostProcessTypes.GetName(PostProcessType.NoProcess) },
                    { PostProcessType.OpenFile, PostProcessTypes.GetName(PostProcessType.OpenFile) },
                    { PostProcessType.OpenFolder, PostProcessTypes.GetName(PostProcessType.OpenFolder) },
                };
            }
        }
        /// <summary>
        /// 選択中の出力後処理Key
        /// </summary>
        public PostProcessType SelectedPostProcessKey
        {
            get { return GeneralSetting.PostProcess; }
            set
            {
                if (value != GeneralSetting.PostProcess)
                {
                    GeneralSetting.PostProcess = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中の出力後処理Value
        /// </summary>
        public string SelectedPostProcessValue
        {
            get { return PostProcessItemSource[SelectedPostProcessKey]; }
        }

        /// <summary>
        /// 上書き設定の選択肢
        /// </summary>
        public Dictionary<OverWriteType, string> OverWriteItemSource
        {
            get
            {
                return new Dictionary<OverWriteType, string>
                {
                    { OverWriteType.Always, OverWriteTypes.GetName(OverWriteType.Always) },
                    { OverWriteType.WithDate, OverWriteTypes.GetName(OverWriteType.WithDate) },
                    { OverWriteType.WithNumber, OverWriteTypes.GetName(OverWriteType.WithNumber) },
                    { OverWriteType.Message, OverWriteTypes.GetName(OverWriteType.Message) },
                };
            }
        }
        /// <summary>
        /// 選択中の出力後処理Key
        /// </summary>
        public OverWriteType SelecteOverWriteKey
        {
            get { return GeneralSetting.OverWrite; }
            set
            {
                if (value != GeneralSetting.OverWrite)
                {
                    GeneralSetting.OverWrite = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 選択中の出力後処理Value
        /// </summary>
        public string SelecteOverWriteValue
        {
            get { return OverWriteItemSource[SelecteOverWriteKey]; }
        }

        /// <summary>
        /// データなし時に出力するか
        /// </summary>
        public bool? IsOutputNonData
        {
            get { return GeneralSetting.IsNoDataOutPut; }
            set
            {
                if (value != GeneralSetting.IsNoDataOutPut)
                {
                    if (value == true)
                    { GeneralSetting.IsNoDataOutPut = true; }
                    else
                    { GeneralSetting.IsNoDataOutPut = false; }
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// データなし時に出力するか
        /// </summary>
        public bool? IsGetQuerynData
        {
            get { return GeneralSetting.IsQueryData; }
            set
            {
                if (value != GeneralSetting.IsQueryData)
                {
                    if (value == true)
                    { GeneralSetting.IsQueryData = true; }
                    else
                    { GeneralSetting.IsQueryData = false; }
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// 各コントロールへ変更を通知します
        /// </summary>
        public void NotifyProperty()
        {
            var properties =
                AppHelper.GetMyProperties<ReportGeneralViewModel>();
            foreach (var name in from info in properties
                                 select info.Name)
            {
                NotifyPropertyChanged(name);
            }
        }
    }
}
