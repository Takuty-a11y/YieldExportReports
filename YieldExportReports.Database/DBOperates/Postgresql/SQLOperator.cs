using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBObjects;

namespace YieldExportReports.Database.DBOperates.Postgresql
{
    public class SQLOperator : ISQLOperator
    {
        public SQLOperator() { }

        /// <summary>
        /// 接続したDBのオブジェクトを取得
        /// </summary>
        /// <param name="serverName">サーバー名</param>
        /// <param name="dbConnection">接続DBオブジェクト</param>
        public DBObjectCollection GetDBObjectsToList(IDbConnection dbConnection)
        {
            var dbObjectCollection = new DBObjectCollection();

            try
            {
                var c = (NpgsqlConnection)dbConnection;

                var dtTbl = c.GetSchema("Tables");
                if (dtTbl == null || dtTbl.Rows.Count <= 0)
                { throw new ArgumentNullException("接続データベースのテーブル"); }

                //データベース
                var dbObj = new DBObject
                {
                    Type = DBObjectType.DataBase,
                    Name = GetTableValue(dtTbl.Rows[0], "TABLE_CATALOG")
                };

                //テーブル
                var dtTblRows = dtTbl.Select("TABLE_TYPE='BASE TABLE'", "TABLE_NAME ASC");
                foreach (DataRow drTbl in dtTblRows)
                {
                    var dbObjTbl = new DBObject
                    {
                        Name = GetTableValue(drTbl, "TABLE_NAME"),
                        Type = DBObjectType.Table
                    };
                    dbObjTbl.Children = GetTableKeys(dbObj.Name, dbObjTbl.Name, c);

                    //列
                    var dtCol =
                        c.GetSchema("Columns", new string[3] { string.Empty, string.Empty, dbObjTbl.Name });

                    var dtColRows = dtCol.Select(null, "ORDINAL_POSITION ASC");
                    foreach (DataRow drCol in dtColRows)
                    {
                        var dbObjCol = new DBObject();
                        dbObjCol.Name = GetTableValue(drCol, "COLUMN_NAME");
                        var dbObjKey =
                                dbObjTbl.Children.FirstOrDefault(x => x.Name == dbObjCol.Name);
                        if (dbObjKey == null || dbObjKey.Equals(default(DBObject)))
                        {
                            dbObjCol.Type = DBObjectType.Field;
                            dbObjCol.FieldInfo.IsAllowNull = GetTableValue(drCol, "IS_NULLABLE") == "YES";
                            dbObjCol.FieldInfo.DataType = GetTableValue(drCol, "DATA_TYPE");
                            dbObjTbl.Children.Add(dbObjCol);
                        }
                        else
                        {
                            dbObjKey.FieldInfo.IsAllowNull = GetTableValue(drCol, "IS_NULLABLE") == "YES";
                            dbObjKey.FieldInfo.DataType = GetTableValue(drCol, "DATA_TYPE");
                        }
                    }
                    dbObj.Children.Add(dbObjTbl);
                }
                dbObjectCollection.Add(dbObj);

                return dbObjectCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return new DBObjectCollection();
            }
        }

        private List<DBObject> GetTableKeys(string svrName, string tblName, NpgsqlConnection c)
        {
            var lstDbObject = new List<DBObject>();

            try
            {
                // 主キー
                var sQuery = PostgreSQLQuery.TablePrimaryKey(svrName, tblName);
                var dataSet = new DataSet();
                using (var dataCommand = new NpgsqlCommand(sQuery, c))
                {
                    dataCommand.CommandTimeout = 60;
                    var dataAdapter = new NpgsqlDataAdapter(dataCommand);
                    dataAdapter.Fill(dataSet);
                }
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var dbObjPK = new DBObject
                    {
                        Name = GetTableValue(row, "COLUMN_NAME"),
                        Type = DBObjectType.Field
                    };
                    dbObjPK.FieldInfo.IsPrimaryKey = true;
                    lstDbObject.Add(dbObjPK);
                }

                // 外部キー
                sQuery = PostgreSQLQuery.TableForeignKey(svrName, tblName);
                dataSet = new DataSet();
                using (var dataCommand = new NpgsqlCommand(sQuery, c))
                {
                    dataCommand.CommandTimeout = 60;
                    var dataAdapter = new NpgsqlDataAdapter(dataCommand);
                    dataAdapter.Fill(dataSet);
                }
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var dbObjFK = new DBObject();
                    dbObjFK.Name = GetTableValue(row, "COLUMN_NAME");
                    var dbObj = lstDbObject.FirstOrDefault(x => x.Name == dbObjFK.Name);
                    if (dbObj == null || dbObj.Equals(default(DBObject)))
                    {
                        dbObjFK.Type = DBObjectType.Field;
                        dbObjFK.FieldInfo.IsForeignKey = true;
                        dbObjFK.FieldInfo.ReferenceTable = GetTableValue(row, "REFERENCE_TABLE_NAME");
                        dbObjFK.FieldInfo.ReferenceColumn = GetTableValue(row, "REFERENCE_COLUMN_NAME");
                        lstDbObject.Add(dbObjFK);
                    }
                    else
                    {
                        dbObj.FieldInfo.IsForeignKey = true;
                        dbObj.FieldInfo.ReferenceTable = GetTableValue(row, "REFERENCE_TABLE_NAME");
                        dbObj.FieldInfo.ReferenceColumn = GetTableValue(row, "REFERENCE_COLUMN_NAME");
                    }
                }
                return lstDbObject;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// テーブルのセルの値を取得します
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string GetTableValue(DataRow row, string columnName) => row[columnName]?.ToString() ?? string.Empty;

        /// <summary>
        /// クエリエディターのSQLを接続DBで実行します
        /// </summary>
        /// <param name="queryText">クエリ文</param>
        /// <param name="dbConnectionString">接続文字列</param>
        /// <param name="token">タスクキャンセルトークン</param>
        /// <returns>List<DataTable></returns>
        public async Task<List<DataTable>> GetQueryDataToDataTables
            (string queryText, string dbConnectionString, CancellationToken token)
        {
            NpgsqlCommand dataCommand;
            List<DataTable> dataTableCollection = new List<DataTable>();
            using (var c = new NpgsqlConnection(dbConnectionString))
            {
                try
                {
                    await c.OpenAsync(token);

                    dataCommand = new NpgsqlCommand(queryText, c);

                    using (var reader = await dataCommand.ExecuteReaderAsync(token))
                    {
                        if (reader == null) throw new ArgumentNullException(nameof(reader));
                        if (reader.IsClosed) return dataTableCollection;

                        var dtRet = new DataTable();
                        await Task.Run(() =>
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }
                                var colType = reader.GetFieldType(i);
                                var colName = reader.GetName(i);
                                dtRet.Columns.Add(colName, colType);
                            }

                        }, token);
                        while (await reader.ReadAsync(token))
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }

                            var drNew = dtRet.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                drNew[i] = reader[i];
                            }
                            dtRet.Rows.Add(drNew);

                            if (reader.IsClosed) break;
                        }
                        dataTableCollection.Add(dtRet);
                    }
                    return dataTableCollection;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "エラー",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return new List<DataTable>();
                }
            }
        }
    }
}
