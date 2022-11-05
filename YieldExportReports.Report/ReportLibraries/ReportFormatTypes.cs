namespace YieldExportReports.Report.ReportLibraries
{
    public enum ReportFormatType
    {
        Excel = 0,
        PDF = 1,
        TEXT = 2,
        HTML = 3
    }

    public class ReportFormatTypes
    {
        public static string GetName(ReportFormatType enmType)
        {
            switch (enmType)
            {
                case ReportFormatType.Excel:
                    return "Excelファイル";
                case ReportFormatType.PDF:
                    return "PDFファイル[追加予定]";
                case ReportFormatType.TEXT:
                    return "Textファイル[追加予定]";
                case ReportFormatType.HTML:
                    return "HTMLファイル[追加予定]";
                default:
                    return string.Empty;
            }
        }
    }
}
