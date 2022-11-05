namespace YieldExportReports.Report.ReportLibraries
{
    public enum OverWriteType
    {
        Always = 0,
        WithDate = 1,
        WithNumber = 2,
        Message = 3,
    }

    public class OverWriteTypes
    {
        public static string GetName(OverWriteType enmType)
        {
            switch (enmType)
            {
                case OverWriteType.Always:
                    return "常に上書き";
                case OverWriteType.WithDate:
                    return "日時付きで保存";
                case OverWriteType.WithNumber:
                    return "番号付きで保存";
                case OverWriteType.Message:
                    return "確認メッセージ表示";
                default:
                    return string.Empty;
            }
        }
    }
}
