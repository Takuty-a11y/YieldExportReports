using System;
using System.Collections.Generic;
using YieldExportReports.Report.ReportFields;
using YieldExportReports.Report.ReportSettings;

namespace YieldExportReports.Report.ExportReports
{
    [Serializable]
    public class ExportReport
    {
        /// <summary>
        /// クエリデータ
        /// </summary>
        public string SQLText { get; set; } = string.Empty;

        /// <summary>
        /// フィールドデータ
        /// </summary>
        public List<ReportField> ResultFields { get; set; }
            = new List<ReportField>();

        /// <summary>
        /// 全般設定データ
        /// </summary>
        public ReportGeneralSetting GeneralSetting { get; set; }
            = new ReportGeneralSetting();

        /// <summary>
        /// テンプレートデータ
        /// </summary>
        public ReportTemplateSetting TemplateSetting { get; set; }
            = new ReportTemplateSetting();

        /// <summary>
        /// ファイル分割データ
        /// </summary>
        public List<ReportSetting> FileSplitData { get; set; }
            = new List<ReportSetting>();

        /// <summary>
        /// シート分割データ
        /// </summary>
        public List<ReportSetting> SheetSplitData { get; set; }
            = new List<ReportSetting>();

        /// <summary>
        /// セル出力データ
        /// </summary>
        public List<ReportSetting> CellOutputData { get; set; }
            = new List<ReportSetting>();

        /// <summary>
        /// 連続出力開始行
        /// </summary>
        public int RepeatStartRow { get; set; }

        /// <summary>
        /// 連続出力終了行
        /// </summary>
        public int RepeatEndRow { get; set; }

        /// <summary>
        /// 連続出力データ
        /// </summary>
        public List<ReportSetting> RepeatOutputData { get; set; }
            = new List<ReportSetting>();
    }
}
