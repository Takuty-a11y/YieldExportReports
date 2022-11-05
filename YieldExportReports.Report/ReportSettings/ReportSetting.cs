using System;
using YieldExportReports.Report.ReportFields;

namespace YieldExportReports.Report.ReportSettings
{
    [Serializable]
    public class ReportSetting
    {
        public Guid ID { get; set; }
        public ReportField Field { get; set; } = new ReportField();
        public string Cell { get; set; } = string.Empty;
    }
}
