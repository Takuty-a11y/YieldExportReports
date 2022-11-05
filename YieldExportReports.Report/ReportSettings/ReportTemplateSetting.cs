using System;

namespace YieldExportReports.Report.ReportSettings
{
    [Serializable]
    public class ReportTemplateSetting
    {
        /// <summary>
        /// テンプレートパス
        /// </summary>
        public string TemplatePath { get; set; } = string.Empty;

        /// <summary>
        /// 出力先シートインデックス
        /// </summary>
        public int TemplateSheetIndex { get; set; }

        /// <summary>
        /// 一時ファイルパス
        /// </summary>
        public string TemporaryFilePath { get; set; } = string.Empty;

        /// <summary>
        /// 一時シート名
        /// </summary>
        public string TemporarySheetName { get; set; } = string.Empty;
    }
}
