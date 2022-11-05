using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using YieldExportReports.Report.ExportReports;
using YieldExportReports.Report.ReportExceptions;
using YieldExportReports.Report.ReportLibraries;
using YieldExportReports.Report.ReportSettings;

namespace YieldExportReports.Report.ReportOperates
{
    public sealed class ReportOperator : IReportOperator
    {
        private ExportReport m_exportSetting;
        private ReportGeneralSetting m_generalSetting;
        private ReportTemplateSetting m_templateSetting;

        /// <summary>
        /// 連続データ開始行
        /// </summary>
        private int m_RepeatStart
        {
            get { return m_exportSetting.RepeatStartRow - 1; }
        }
        /// <summary>
        /// 連続データ終了行
        /// </summary>
        private int m_RepeatEnd
        {
            get { return m_exportSetting.RepeatEndRow - 1; }
        }
        /// <summary>
        /// 連続行数
        /// </summary>
        private int m_RepeatInterval
        {
            get { return m_exportSetting.RepeatEndRow - m_exportSetting.RepeatStartRow + 1; }
        }
        /// <summary>
        /// 一時ファイルパス
        /// </summary>
        private string m_TemporaryPath
        {
            get
            {
                var path = m_templateSetting.TemporaryFilePath;
                if (!Directory.Exists(path))
                { path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? @"C:\"; }

                return Path.Combine(path, Path.GetFileName(m_templateSetting.TemplatePath));
            }
        }
        /// <summary>
        /// 一時シート名
        /// </summary>
        private string m_TemporarySheet
        {
            get
            {
                var name = m_templateSetting.TemporarySheetName;
                if (string.IsNullOrWhiteSpace(name))
                { name = "Report"; }

                return name;
            }
        }

        public ReportOperator(ExportReport exportReport)
        {
            m_exportSetting = exportReport;
            m_generalSetting = exportReport.GeneralSetting;
            m_templateSetting = exportReport.TemplateSetting;
        }

        /// <summary>
        /// レポートを出力します
        /// </summary>
        /// <param name="dataSource">出力元データ</param>
        /// <returns>出力済パスリスト</returns>
        public List<string> RunExport(DataTable dataSource)
        {
            var targetData = dataSource ?? new DataTable();
            var fileNameList = new Dictionary<string, List<string>>();
            var workBookList = new Dictionary<string, IWorkbook>();

            try
            {
                var preFileName = string.Empty;
                var preSheetName = string.Empty;
                var templateSheetName = string.Empty;
                var temporarySheetName = string.Empty;
                var repeatIndex = 0;
                ISheet? nowSheet = null;

                //出力前確認
                CheckSettingValue(targetData);

                //一時ファイル作成
                File.Copy(m_templateSetting.TemplatePath, m_TemporaryPath);

                foreach (DataRow row in targetData.Rows)
                {
                    //ファイル分割
                    var splitFileName = string.Empty;
                    if (IsSplit(ReportSplitType.File))
                    { splitFileName = GetSplitFileName(row, targetData.Columns); }
                    else
                    { splitFileName = ExcelUtilities.RemoveFileSpecial(m_generalSetting.FileName); }

                    //ファイル追加
                    var isExistFile = fileNameList.ContainsKey(splitFileName);
                    if (!isExistFile)
                    {
                        fileNameList.Add(splitFileName, new List<string>());

                        using (var stream = new FileStream(
                                                    m_TemporaryPath,
                                                    FileMode.Open,
                                                    FileAccess.Read,
                                                    FileShare.ReadWrite))
                        {
                            var newBook = WorkbookFactory.Create(stream, false);

                            //一時シート作成
                            templateSheetName =
                                newBook.GetSheetName(m_templateSetting.TemplateSheetIndex)
                                    ?? throw new DataNotExistsException("テンプレートシート");
                            temporarySheetName = GetSheetNameWithEscape($"#@#@{m_TemporarySheet}#@#@");
                            newBook.SetSheetName(m_templateSetting.TemplateSheetIndex, temporarySheetName);

                            workBookList.Add(splitFileName, newBook);
                        }
                    }

                    //シート分割
                    var splitSheetName = templateSheetName;
                    if (IsSplit(ReportSplitType.Sheet))
                    { splitSheetName = GetSplitSheetName(row, targetData.Columns, templateSheetName); }

                    if (string.IsNullOrWhiteSpace(splitSheetName))
                    { splitSheetName = m_TemporarySheet; }

                    //シート追加
                    var isExistSheet = fileNameList[splitFileName].Contains(splitSheetName);
                    if (!isExistSheet)
                    {
                        fileNameList[splitFileName].Add(splitSheetName);

                        var sheet = 
                            workBookList[splitFileName]?.GetSheet(temporarySheetName) 
                                ?? throw new DataNotExistsException("一時シート");
                        try
                        { nowSheet = sheet.CopySheet(splitSheetName); }
                        catch (Exception ex)
                        { throw new ActionFailureException($"シート[{splitSheetName}]作成", ex.Message); }
                    }

                    if (nowSheet != null)
                    {
                        //シート初回
                        var isFirstSheet = splitFileName != preFileName;
                        isFirstSheet |= splitSheetName != preSheetName;
                        if (isFirstSheet)
                        {
                            repeatIndex = 0;
                            OutputData(row, m_exportSetting.CellOutputData, nowSheet);
                            OutputData(row, m_exportSetting.RepeatOutputData, nowSheet);
                        }
                        else
                        {
                            var sourceStartRow = m_RepeatStart + repeatIndex - m_RepeatInterval;
                            var sourceEndRow = m_RepeatEnd + repeatIndex - m_RepeatInterval;
                            var isXLS = workBookList[splitFileName].SpreadsheetVersion == SpreadsheetVersion.EXCEL97;
                            ExcelUtilities.InsertRow(nowSheet, sourceStartRow, sourceEndRow, isXLS);
                            OutputData(row, m_exportSetting.RepeatOutputData, nowSheet, repeatIndex);
                        }
                    }

                    preFileName = splitFileName;
                    preSheetName = splitSheetName;
                    repeatIndex += m_RepeatInterval;
                }

                var pathList = new List<string>();
                foreach (var fileName in from r in fileNameList
                                         where !string.IsNullOrWhiteSpace(r.Key)
                                            && workBookList.ContainsKey(r.Key)
                                         select r.Key)
                {
                    var workBook = workBookList[fileName];
                    var sheetIndex = workBook.GetSheetIndex(temporarySheetName);
                    try
                    { workBook.RemoveSheetAt(sheetIndex); }
                    catch (Exception ex)
                    { throw new ActionFailureException("一時シート削除", ex.Message); }

                    var outputPath = GetPathByOverwriteType(fileName);
                    using (var stream = new FileStream(
                                                outputPath,
                                                FileMode.Create,
                                                FileAccess.Write,
                                                FileShare.None))
                    {
                        try
                        { workBook.Write(stream); }
                        catch (Exception ex)
                        { throw new ActionFailureException("ファイル書き込み", ex.Message); }
                    }

                    pathList.Add(outputPath);
                }
                return pathList.Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                foreach (var workBook in from r in workBookList
                                         where r.Value != null
                                         select r.Value)
                { workBook.Close(); }

                if (File.Exists(m_TemporaryPath))
                {
                    // 読み取り専用解除⇒削除
                    var fileInfo = new FileInfo(m_TemporaryPath);
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fileInfo.Attributes = FileAttributes.Normal;
                    }
                    fileInfo.Delete();
                }
            }
        }

        /// <summary>
        /// 入力値に不正がないか確認します
        /// </summary>
        /// <param name="dataTable"></param>
        private void CheckSettingValue(DataTable dataTable)
        {
            if (dataTable.Rows.Count <= 0
                && m_generalSetting.IsNoDataOutPut)
            { throw new DataNotExistsException("レポート出力データ"); }

            if (!File.Exists(m_templateSetting.TemplatePath))
            { throw new DataNotExistsException("テンプレートファイル", m_templateSetting.TemplatePath); }

            if (!Directory.Exists(m_generalSetting.ExportPath))
            { throw new DataNotExistsException("レポート出力先", m_generalSetting.ExportPath); }

            foreach (var item in m_exportSetting.CellOutputData)
            {
                var cellAddress = new CellReference(item.Cell);
                if (!ExcelUtilities.IsCell(cellAddress))
                {
                    throw new ValueIncorrectException(
                        "セル出力設定",
                        new string[] { $"{item.Field.Name}：{item.Cell}" });
                }
            }

            foreach (var item in m_exportSetting.RepeatOutputData)
            {
                var cellAddress = new CellReference(item.Cell);
                if (!ExcelUtilities.IsCell(cellAddress))
                {
                    throw new ValueIncorrectException(
                        "連続出力設定",
                        new string[] { $"{item.Field.Name}：{item.Cell}" });
                }

                if (cellAddress.Row < m_RepeatStart
                        || cellAddress.Row > m_RepeatEnd)
                {
                    throw new ValueIncorrectException(
                        "連続出力の範囲",
                        new string[]
                        {
                            $"連続範囲：{m_RepeatStart + 1}～{m_RepeatEnd + 1}",
                            $"{item.Field.Name}：{item.Cell}"
                        });
                }
            }

            if (m_RepeatEnd < m_RepeatStart)
            {
                throw new ValueIncorrectException(
                    "連続出力開始～終了",
                    new string[] { $"開始行：{m_RepeatStart + 1}", $"終了行{m_RepeatEnd + 1}" });
            }
        }

        /// <summary>
        /// 分割ファイル名を取得します
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetSplitFileName(DataRow row, DataColumnCollection columns)
        {
            var retName =
                GetSplitName(
                    row,
                    columns,
                    m_generalSetting.FileName,
                    m_exportSetting.FileSplitData);

            return ExcelUtilities.RemoveFileSpecial(retName);
        }

        /// <summary>
        /// 分割シート名を取得します
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetSplitSheetName(DataRow row, DataColumnCollection columns, string sheetName)
        {
            var retName =
                GetSplitName(
                    row,
                    columns,
                    sheetName,
                    m_exportSetting.SheetSplitData);

            return GetSheetNameWithEscape(retName);
        }
        /// <summary>
        /// 文字数制限したシート名を取得します
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private string GetSheetNameWithEscape(string sheetName)
        {
            //特殊文字
            sheetName = ExcelUtilities.RemoveSheetSpecial(sheetName);
            //最大文字数
            return ExcelUtilities.RemoveSheetMaximum(sheetName);
        }

        /// <summary>
        /// 分割設定を元に名称を決定します
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="defaultName"></param>
        /// <param name="reportSettings"></param>
        /// <returns></returns>
        private string GetSplitName
            (DataRow row, DataColumnCollection columns, string defaultName, List<ReportSetting> reportSettings)
        {
            var retName = defaultName;
            foreach (var field in from r in reportSettings
                                  where r.Field != null
                                  select r.Field)
            {
                foreach (DataColumn col in columns)
                {
                    if (field.Name != col.ColumnName)
                    { continue; }

                    retName += "_";
                    retName += DataSourceUtilities.DataTypeString(field.DataType, row[col]);
                }
            }
            return retName;
        }

        /// <summary>
        /// データソースからセルに出力します
        /// </summary>
        /// <param name="row"></param>
        /// <param name="reportSettings"></param>
        /// <param name="sheet"></param>
        /// <param name="repeatIndex"></param>
        private void OutputData
            (DataRow row, List<ReportSetting> reportSettings, ISheet sheet, int repeatIndex = 0)
        {
            var iemSetting = from r in reportSettings
                             where !string.IsNullOrWhiteSpace(r.Cell)
                                     && r.Field != null
                             select new
                             {
                                 dataType = r.Field.DataType,
                                 fieldName = r.Field.Name,
                                 cell = r.Cell
                             };
            foreach (var setting in iemSetting)
            {
                try
                {
                    var sourceValue =
                        DataSourceUtilities.DataTypeString(setting.dataType, row[setting.fieldName]);
                    var cellAddress = new CellReference(setting.cell);
                    ExcelUtilities.WriteCell
                        (sheet, cellAddress.Col, cellAddress.Row + repeatIndex, sourceValue.ToString());
                }
                catch
                {
                    throw new Exception("データの出力に失敗しました。");
                }
            }
        }

        /// <summary>
        /// 分割実行判定
        /// </summary>
        /// <param name="splitType">ファイルorシート</param>
        /// <returns></returns>
        private bool IsSplit(ReportSplitType splitType)
        {
            var bRet = m_generalSetting.SplitType == splitType;
            bRet |= m_generalSetting.SplitType == ReportSplitType.FileSheet;
            return bRet;
        }

        /// <summary>
        /// 上書き設定による出力パスを取得します
        /// </summary>
        /// <param name="fileName">出力ファイル名</param>
        /// <returns>出力パス</returns>
        private string GetPathByOverwriteType(string fileName)
        {
            var extension = Path.GetExtension(m_templateSetting.TemplatePath);

            if (!File.Exists(Path.Combine(m_generalSetting.ExportPath, fileName + extension)))
            { return Path.Combine(m_generalSetting.ExportPath, fileName + extension); }

            var newFileName = fileName;

            Action addNumberToName = () =>
            {
                for (int i = 1; i <= 100; i++)
                {
                    newFileName = $"{fileName}_{i}";
                    if (!File.Exists(Path.Combine(m_generalSetting.ExportPath, newFileName + extension)))
                    { break; }
                }
            };

            switch (m_generalSetting.OverWrite)
            {
                case OverWriteType.Message:
                    var message = new StringBuilder();
                    message.AppendLine($"ファイル[{fileName}]は既に存在しています。");
                    message.AppendLine("ファイルを上書きしますか？");
                    if (MessageBox.Show(
                            message.ToString(),
                            "上書き確認",
                            MessageBoxButton.OKCancel
                            , MessageBoxImage.Information)
                        != MessageBoxResult.OK)
                    { addNumberToName(); }
                    break;

                case OverWriteType.WithDate:
                    var isFileExist = true;
                    while (isFileExist)
                    {
                        var nowDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                        newFileName = $"{fileName}_{nowDate}";

                        isFileExist = File.Exists(Path.Combine(m_generalSetting.ExportPath, newFileName + extension));
                    }
                    break;

                case OverWriteType.WithNumber:
                    addNumberToName();
                    break;
            }

            return Path.Combine(m_generalSetting.ExportPath, newFileName + extension);
        }
    }
}
