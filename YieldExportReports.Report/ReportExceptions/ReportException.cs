using System;

namespace YieldExportReports.Report.ReportExceptions
{
    public abstract class ReportException : Exception
    {
        public ReportException(string message)
            : base(message)
        {
        }

        public ReportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public virtual string Parameter { get; set; } = string.Empty;

        public abstract ExceptionKind Kind { get; }

        public enum ExceptionKind
        {
            Information = 0,
            Warning = 1,
            Error = 2,
        }
    }
}
