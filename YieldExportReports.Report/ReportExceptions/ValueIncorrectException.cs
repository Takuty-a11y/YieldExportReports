using System.Text;

namespace YieldExportReports.Report.ReportExceptions
{
    public sealed class ValueIncorrectException : ReportException
    {
        public ValueIncorrectException(string title, string[] parameters)
            : base($"{title}が不正です。")
        {
            var sbParams = new StringBuilder();
            foreach (var param in parameters)
            {
                sbParams.AppendLine(param);
            }
            Parameter = sbParams.ToString();
        }

        public override ExceptionKind Kind => ExceptionKind.Error;
    }
}
