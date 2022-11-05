namespace YieldExportReports.Report.ReportExceptions
{
    public sealed class DataNotExistsException : ReportException
    {
        public DataNotExistsException(string title, string parameter = "")
            : base($"{title}が見つかりませんでした。")
        {
            Parameter = $"{parameter}";
        }

        public override ExceptionKind Kind => ExceptionKind.Warning;
    }
}
