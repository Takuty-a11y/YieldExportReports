namespace YieldExportReports.Report.ReportLibraries
{
    public enum ReportSplitType
    {
        NoSplit = 0,
        File = 1,
        Sheet = 2,
        FileSheet = 3,
    }

    public class ReportSplitTypes
    {
        public static string GetName(ReportSplitType enmType)
        {
            switch (enmType)
            {
                case ReportSplitType.NoSplit:
                    return "分割しない";
                case ReportSplitType.File:
                    return "ファイル分割のみ";
                case ReportSplitType.Sheet:
                    return "シート分割のみ";
                case ReportSplitType.FileSheet:
                    return "ファイル&シート分割";
                default:
                    return string.Empty;
            }
        }
    }
}
