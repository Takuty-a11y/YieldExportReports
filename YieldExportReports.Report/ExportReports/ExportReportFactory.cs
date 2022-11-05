using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using YieldExportReports.Report.ReportOperates;

namespace YieldExportReports.Report.ExportReports
{
    public static class ExportReportFactory
    {
        public static IReportOperator CreateOperator(ExportReport exportReport)
        {
            return new ReportOperator(exportReport);
        }

        public static void Serialize(ExportReport exportReport, string filePath)
        {
            try
            {
                var xs = new XmlSerializer(typeof(ExportReport));
                using (var writer = new StreamWriter(filePath))
                {
                    xs.Serialize(writer, exportReport);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static ExportReport Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new ExportReport();
            }

            var xs = new XmlSerializer(typeof(ExportReport));
            using (var reader = new StreamReader(filePath))
            {
                return xs.Deserialize(reader) as ExportReport ?? new ExportReport();
            }
        }

        public static Dictionary<int, string> GetTemplateSheets(string filePath)
        {
            try
            {
                var sheetList = new Dictionary<int, string>();
                IWorkbook workBook;
                using (var stream = new FileStream(
                                            filePath,
                                            FileMode.Open,
                                            FileAccess.Read,
                                            FileShare.ReadWrite))
                {
                    workBook = WorkbookFactory.Create(stream, true)
                        ?? throw new ArgumentNullException(nameof(workBook));
                }
                for (var i = 0; i < workBook.NumberOfSheets; i++)
                {
                    var sheetName = workBook.GetSheetName(i);
                    if (string.IsNullOrWhiteSpace(sheetName))
                    { continue; }

                    sheetList.Add(i, sheetName);
                }

                return sheetList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
