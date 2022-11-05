using Microsoft.WindowsAPICodePack.Dialogs;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YieldExportReports.Report.ReportLibraries;

namespace YieldExportReports.Report.ExportFiles
{
    public static class ExportFileFactory
    {
        /// <summary>
        /// DatatableをCSVファイルに保存する
        /// </summary>
        /// <param name="dt">出力対象のDataTable</param>
        /// <param name="qcols">ダブルクォートで括りたい列の番号 int[]{1,5,7} </param>
        /// <param name="append">追加書き込みモード  false:上書き true:追加　省略時はfalse</param>
        /// <param name="encode">エンコード文字列：省略時はshift-jis</param>
        public static void CSV
            (DataTable dt, int[]? qcols = null, bool append = false, string encode = "shift-jis")
        {
            Func<IEnumerable<object?>, int[]?, string?[]> DoubleQuote = (pItem, pQcols) =>
            {
                int cnt = 0;
                return pItem.Select(i =>
                                    (pQcols != null && pQcols.Contains(cnt++))
                                        ? "\"" + i?.ToString()?.Replace("\"", "\"\"") + "\""
                                        : i?.ToString()).ToArray();
            };

            try
            {
                var fileName = GetFileSavePath("csv");
                if (string.IsNullOrWhiteSpace(fileName)) return;

                using (var sw = new StreamWriter(fileName, append, Encoding.GetEncoding(encode)))
                {
                    string dquote = (qcols == null) ? "" : "\"";
                    sw.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>()
                                 .Select(i => dquote + i.ColumnName + dquote).ToArray()));
                    foreach (DataRow dr in dt.Rows)
                    {
                        sw.WriteLine(string.Join(",", DoubleQuote(dr.ItemArray, qcols)));
                    }
                }
            }
            catch (SystemException ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// DatatableをXMLファイルに保存する
        /// </summary>
        /// <param name="dt">出力対象のDataTable</param>
        /// <param name="append">追加書き込みモード  false:上書き true:追加　省略時はfalse</param>
        /// <param name="encode">エンコード文字列：省略時はshift-jis</param>
        public static void XML
            (DataTable dt, bool append = false, string encode = "shift-jis")
        {
            try
            {
                var fileName = GetFileSavePath("xml");
                if (string.IsNullOrWhiteSpace(fileName)) return;

                using (var sw = new StreamWriter(fileName, append, Encoding.GetEncoding(encode)))
                {
                    dt.TableName = "row";
                    dt.WriteXml(sw);
                }
            }
            catch (SystemException ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// DatatableをJSONファイルに保存する
        /// </summary>
        /// <param name="dt">出力対象のDataTable</param>
        /// <param name="append">追加書き込みモード  false:上書き true:追加　省略時はfalse</param>
        /// <param name="encode">エンコード文字列：省略時はshift-jis</param>
        public static void JSON
            (DataTable dt, bool append = false, string encode = "shift-jis")
        {
            try
            {
                var fileName = GetFileSavePath("json");
                if (string.IsNullOrWhiteSpace(fileName)) return;

                var JSONString = new StringBuilder();
                using (var sw = new StreamWriter(fileName, append, Encoding.GetEncoding(encode)))
                {
                    JSONString.AppendLine("[");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        JSONString.AppendLine("\t{");
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j < dt.Columns.Count - 1)
                            {
                                JSONString.AppendLine("\t\t\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                            }
                            else if (j == dt.Columns.Count - 1)
                            {
                                JSONString.AppendLine("\t\t\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                            }
                        }
                        if (i == dt.Rows.Count - 1)
                        {
                            JSONString.AppendLine("\t}");
                        }
                        else
                        {
                            JSONString.AppendLine("\t},");
                        }
                    }
                    JSONString.AppendLine("]");
                    sw.WriteLine(JSONString.ToString());
                }
            }
            catch (SystemException ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// DatatableをExcelファイルに保存する
        /// </summary>
        /// <param name="dt">出力対象のDataTable</param>
        /// <param name="append">追加書き込みモード  false:上書き true:追加　省略時はfalse</param>
        /// <param name="encode">エンコード文字列：省略時はshift-jis</param>
        public static void EXCEL
            (DataTable dt, bool append = false, string encode = "shift-jis")
        {
            var workBook = new XSSFWorkbook();
            var sheet = workBook.CreateSheet("Result");
            try
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var col = dt.Columns[i];
                    ExcelUtilities.WriteCell(sheet, i, 0, col.ColumnName);
                }

                for (int rowIndex = 1; rowIndex <= dt.Rows.Count; rowIndex++)
                {
                    var row = dt.Rows[rowIndex - 1];

                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                    {
                        var col = dt.Columns[colIndex];
                        if (row[col] == DBNull.Value || row[col] == null)
                        {
                            ExcelUtilities.WriteCell(sheet, colIndex, rowIndex, string.Empty);
                            continue;
                        }

                        switch (col.DataType.Name)
                        {
                            case "Integer":
                            case "Int32":
                            case "Decimal":
                            case "Long":
                            case "Double":
                            case "Short":
                                var dVal = Convert.ToDouble(row[col]);
                                ExcelUtilities.WriteCell(sheet, colIndex, rowIndex, dVal);
                                break;
                            case "DateTime":
                                var dtVal = Convert.ToDateTime(row[col]);
                                ExcelUtilities.WriteCell(sheet, colIndex, rowIndex, dtVal);

                                var style = workBook.CreateCellStyle();
                                style.DataFormat = workBook.CreateDataFormat().GetFormat("yyyy/mm/dd");
                                ExcelUtilities.WriteStyle(sheet, colIndex, rowIndex, style);
                                break;
                            default:
                                var sVal = row[col].ToString() ?? string.Empty;
                                ExcelUtilities.WriteCell(sheet, colIndex, rowIndex, sVal);
                                break;
                        }
                    }
                }

                var fileName = GetFileSavePath("xlsx");
                if (string.IsNullOrWhiteSpace(fileName)) return;

                using (var fs = new FileStream(fileName, FileMode.Create))
                { workBook.Write(fs); }
            }
            catch (SystemException ex)
            { ShowErrorMessage(ex); }
            finally
            { workBook.Close(); }
        }

        private static string GetFileSavePath(string sType)
        {
            var dlgSave = new CommonSaveFileDialog()
            {
                Title = "保存先ファイル選択",
            };
            var filter = new CommonFileDialogFilter(sType.ToUpper() + "ファイル", "*." + sType);
            dlgSave.Filters.Add(filter);

            if (dlgSave.ShowDialog() == CommonFileDialogResult.Ok)
            { return dlgSave.FileName; }
            else
            { return string.Empty; }
        }

        private static void ShowErrorMessage(SystemException ex)
        {
            MessageBox.Show(
                ex.Message,
                "ファイル出力エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
