using System;
using YieldExportReports.Report.ReportLibraries;

namespace YieldExportReports.Report.ReportSettings
{
    [Serializable]
    public class ReportGeneralSetting
    {
        /// <summary>
        /// 出力場所
        /// </summary>
        public string ExportPath { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 出力形式
        /// </summary>
        public ReportFormatType FormatType { get; set; }

        /// <summary>
        /// 分割設定
        /// </summary>
        public ReportSplitType SplitType { get; set; }

        /// <summary>
        /// 出力後処理
        /// </summary>
        public PostProcessType PostProcess { get; set; }

        /// <summary>
        /// 上書き設定
        /// </summary>
        public OverWriteType OverWrite { get; set; }

        /// <summary>
        /// データなし時の出力可否
        /// </summary>
        public bool IsNoDataOutPut { get; set; }

        /// <summary>
        /// クエリからのデータ取得可否
        /// </summary>
        public bool IsQueryData { get; set; }
    }
}
