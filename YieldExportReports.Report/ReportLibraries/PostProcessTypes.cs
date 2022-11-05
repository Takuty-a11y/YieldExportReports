namespace YieldExportReports.Report.ReportLibraries
{
    public enum PostProcessType
    {
        NoProcess = 0,
        OpenFile = 1,
        OpenFolder = 2,
    }

    public class PostProcessTypes
    {
        public static string GetName(PostProcessType enmType)
        {
            switch (enmType)
            {
                case PostProcessType.NoProcess:
                    return "何もしない";
                case PostProcessType.OpenFile:
                    return "ファイルを開く";
                case PostProcessType.OpenFolder:
                    return "フォルダを開く";
                default:
                    return string.Empty;
            }
        }
    }
}
