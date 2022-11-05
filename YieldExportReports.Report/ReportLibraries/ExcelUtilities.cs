using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YieldExportReports.Report.ReportLibraries
{
    public static class ExcelUtilities
    {
        /// <summary>
        /// 新規ワークブックを作成します
        /// </summary>
        /// <param name="filePath">ファイル名</param>
        /// <returns>新規ワークブック</returns>
        public static IWorkbook CreateWorkBook(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (extension == "xls")
            { return new HSSFWorkbook(); }
            else if (extension == "xlsx")
            { return new XSSFWorkbook(); }
            else if (extension == "xlsm")
            { return new XSSFWorkbook(); }
            else
            { throw new ApplicationException("CreateNewBook: invalid extension"); }
        }

        /// <summary>
        /// 行挿入
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="fromRow"></param>
        /// <param name="toRow"></param>
        public static void InsertRow(ISheet sheet, int fromRow, int toRow, bool isXLS)
        {
            var countRow = toRow - fromRow + 1;
            var formulaList = new Dictionary<CellReference, string>();
            try
            {
                //Get Formula
                for (var i = fromRow; i < fromRow + countRow; i++)
                {
                    var sourceRow = sheet.GetRow(i);
                    if (sourceRow == null) continue;

                    for (int j = sourceRow.FirstCellNum; j < sourceRow.LastCellNum; j++)
                    {
                        var sourceCell = sourceRow.GetCell(j);
                        if (sourceCell?.CellType == CellType.Formula)
                        {
                            formulaList.Add(new CellReference(i, j), sourceCell.CellFormula);
                        }
                    }
                }

                sheet.ShiftRows(fromRow, sheet.LastRowNum, countRow, true, false);

                //Row Insert
                for (var i = fromRow; i < fromRow + countRow; i++)
                {
                    var sourceRow = sheet.GetRow(i + countRow);
                    if (sourceRow == null) continue;

                    IRow targetRow;
                    if (isXLS)
                    {
                        //XLS
                        targetRow = CopyRow(sheet, i + countRow, i) ?? sheet.CreateRow(i);
                    }
                    else
                    {
                        // XLSX,XLSM
                        targetRow = sheet.CopyRow(i + countRow, i) ?? sheet.CreateRow(i);
                        targetRow.Hidden = sourceRow.Hidden;
                        targetRow.Collapsed = sourceRow.Collapsed;
                    }

                    //Set Formula
                    foreach (var cell in from r in formulaList
                                         where r.Key.Row == targetRow.RowNum
                                         select new { col = r.Key.Col, formula = r.Value })
                    {
                        var targetCell = targetRow.GetCell(cell.col) ?? targetRow.CreateCell(cell.col);
                        targetCell.SetCellFormula(cell.formula);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = new StringBuilder();
                message.AppendLine("行挿入に失敗しました。");
                message.AppendLine($"理由：{ex.Message}");

                throw new Exception(message.ToString());
            }
        }

        /// <summary>
        /// [XLS用]行コピー
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        private static IRow CopyRow(ISheet worksheet, int sourceIndex, int targetIndex)
        {
            var sourceRow = worksheet.GetRow(sourceIndex);
            var targetRow = worksheet.GetRow(targetIndex) ?? worksheet.CreateRow(targetIndex);

            // スタイル、セル型をコピーする
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                var sourceCell = sourceRow.GetCell(i);
                var targetCell = targetRow.CreateCell(i);

                // コピー元の行が存在しない場合、処理を中断
                if (sourceCell == null)
                {
                    targetCell = null;
                    continue;
                }

                // スタイルのコピー
                targetCell.CellStyle = sourceCell.CellStyle;

                // 行の高さをコピー
                targetRow.Height = sourceRow.Height;

                // セル型のコピー
                targetCell.SetCellType(sourceCell.CellType);

                // セルの値をコピー
                switch (sourceCell.CellType)
                {
                    case CellType.Blank:
                        targetCell.SetCellValue(sourceCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        targetCell.SetCellValue(sourceCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        targetCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        targetCell.SetCellFormula(sourceCell.CellFormula);
                        break;
                    case CellType.Numeric:
                        targetCell.SetCellValue(sourceCell.NumericCellValue);
                        break;
                    case CellType.String:
                        targetCell.SetCellValue(sourceCell.RichStringCellValue);
                        break;
                }
            }

            // セル結合のコピー
            for (int i = 0; i < worksheet.NumMergedRegions; i++)
            {
                var cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    var newCellRangeAddress = new CellRangeAddress(
                        targetRow.RowNum,
                        targetRow.RowNum + (cellRangeAddress.LastRow - cellRangeAddress.FirstRow),
                        cellRangeAddress.FirstColumn,
                        cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }

            return targetRow;
        }

        /// <summary>
        /// セル設定[文字列]
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        public static void WriteCell(ISheet sheet, int columnIndex, int rowIndex, string value)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(columnIndex) ?? row.CreateCell(columnIndex);

            cell.SetCellValue(value);
            cell.SetCellType(CellType.String);
        }

        /// <summary>
        /// セル設定[数値用]
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        public static void WriteCell(ISheet sheet, int columnIndex, int rowIndex, double value)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(columnIndex) ?? row.CreateCell(columnIndex);

            cell.SetCellValue(value);
            cell.SetCellType(CellType.Numeric);
        }

        /// <summary>
        /// セル設定[日付用]
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        public static void WriteCell(ISheet sheet, int columnIndex, int rowIndex, DateTime value)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(columnIndex) ?? row.CreateCell(columnIndex);

            cell.SetCellValue(value);
        }

        /// <summary>
        /// 書式変更
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="style"></param>
        public static void WriteStyle(ISheet sheet, int columnIndex, int rowIndex, ICellStyle style)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            var cell = row.GetCell(columnIndex) ?? row.CreateCell(columnIndex);

            cell.CellStyle = style;
        }

        /// <summary>
        /// ファイル名の特殊文字を削除します
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <returns></returns>
        internal static string RemoveFileSpecial(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { return name; }

            name = name.Replace("<", "");
            name = name.Replace(">", "");
            name = name.Replace("*", "");
            name = name.Replace("\\", "");
            name = name.Replace(":", "");
            name = name.Replace(",", "");
            name = name.Replace("\"", "");
            name = name.Replace("|", "");
            name = name.Replace("?", "");
            name = name.Replace("/", "");

            return name;
        }

        /// <summary>
        /// シート名の特殊文字を削除します
        /// </summary>
        /// <param name="name">シート名</param>
        /// <returns></returns>
        internal static string RemoveSheetSpecial(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { return name; }

            name = name.Replace("*", "");
            name = name.Replace("\\", "");
            name = name.Replace("[", "");
            name = name.Replace("]", "");
            name = name.Replace(":", "");
            name = name.Replace("?", "");
            name = name.Replace("/", "");

            return name;
        }

        /// <summary>
        /// シート名を最大文字数まで削除します
        /// </summary>
        /// <param name="name">シート名</param>
        /// <param name="maximum">最大文字数</param>
        /// <returns></returns>
        internal static string RemoveSheetMaximum(string name, int maximum = 31)
        {
            if (string.IsNullOrEmpty(name)) { return name; }

            var encoding = Encoding.GetEncoding(932);
            if (encoding.GetByteCount(name) <= maximum) { return name; }

            var bytes = encoding.GetBytes(name);
            return encoding.GetString(bytes, 0, maximum);
        }

        /// <summary>
        /// セルが正しく認識されているか確認します
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        internal static bool IsCell(CellReference cell)
        {
            _ = cell != null;
            _ = cell?.Row >= 0;
            bool retResult = cell?.Col >= 0;

            return retResult;
        }
    }
}
