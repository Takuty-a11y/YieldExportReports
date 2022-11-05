using System.Collections.Generic;
using System.Data;

namespace YieldExportReports.Report.ReportOperates
{
    public interface IReportOperator
    {
        List<string> RunExport(DataTable dtSource);
    }
}
