namespace YieldExportReports.Report.ReportExceptions
{
    public sealed class ActionFailureException : ReportException
    {
        public ActionFailureException(string title, string message)
            : base($"{title}に失敗しました。")
        {
            Parameter = $"理由：{message}";
        }

        public override ExceptionKind Kind => ExceptionKind.Error;
    }
}
