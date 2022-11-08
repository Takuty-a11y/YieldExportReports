using MySqlConnector;
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

namespace YieldExportReports.Database.DBOperates.MySQL
{
    public class SQLOperator : ISQLOperator
    {
        public SQLOperator() { }

        /// <summary>
        /// 接続したDBのオブジェクトを取得
        /// </summary>
        /// <param name="dbConnection">接続DBオブジェクト</param>
        /// <returns>DBObjectCollection</returns>
        public DBObjectCollection GetDBObjectsToList(IDbConnection dbConnection)
        {
            var dbObjectCollection = new DBObjectCollection();

            try
            {
                var c = (MySqlConnection)dbConnection;
                c.InfoMessage += new MySqlInfoMessageEventHandler(c_InfoMessage);

                var serverName = c.Database;
                var iemTable = from r in c.GetSchema("Tables").AsEnumerable()
                               where GetTableValue(r, "TABLE_SCHEMA").Equals(serverName, StringComparison.OrdinalIgnoreCase)
                               select r;

                if (iemTable == null || !iemTable.Any())
                { throw new ArgumentNullException("接続データベースのテーブル"); }                                  

                var dbObj = new DBObject
                {
                    Type = DBObjectType.DataBase,
                    Name = serverName,
                };

                //テーブル
                var iemBaseTable = from r in iemTable
                                   orderby GetTableValue(r, "TABLE_NAME") ascending
                                   where GetTableValue(r, "TABLE_TYPE").Equals("BASE TABLE", StringComparison.OrdinalIgnoreCase)
                                   select r;

                foreach (var drTbl in iemBaseTable)
                {
                    var dbObjTbl = new DBObject
                    {
                        Name = GetTableValue(drTbl, "TABLE_NAME"),
                        Type = DBObjectType.Table
                    };
                    dbObjTbl.Children = GetTableKeys(dbObj.Name, dbObjTbl.Name, c);

                    //列
                    var dtColRows = from r in c.GetSchema("Columns").AsEnumerable()
                                    orderby r["ORDINAL_POSITION"] ascending
                                    where GetTableValue(r, "TABLE_SCHEMA").Equals(serverName, StringComparison.OrdinalIgnoreCase)
                                        && GetTableValue(r, "TABLE_NAME").Equals(dbObjTbl.Name, StringComparison.OrdinalIgnoreCase)
                                    select r;

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

        /// <summary>
        /// テーブルの主キーと外部キー情報を取得します
        /// </summary>
        /// <param name="svrName"></param>
        /// <param name="tblName"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private List<DBObject> GetTableKeys(string svrName, string tblName, MySqlConnection c)
        {
            var lstDbObject = new List<DBObject>();

            try
            {
                var sQuery = MySQLQuery.TableKeyQuery(svrName, tblName);
                var dataSet = new DataSet();
                using (var dataCommand = new MySqlCommand(sQuery, c))
                {
                    dataCommand.CommandTimeout = 60;
                    var dataAdapter = new MySqlDataAdapter(dataCommand);
                    dataAdapter.Fill(dataSet);
                }
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var keyName = row["CONSTRAINT_NAME"].ToString();
                    if (keyName == "PRIMARY")
                    {
                        var dbObjPK = new DBObject
                        {
                            Name = GetTableValue(row, "COLUMN_NAME"),
                            Type = DBObjectType.Field
                        };
                        dbObjPK.FieldInfo.IsPrimaryKey = true;
                        lstDbObject.Add(dbObjPK);
                    }

                    var bForeignKey = row["REFERENCED_TABLE_SCHEMA"] != DBNull.Value;
                    if (bForeignKey)
                    {
                        var dbObjFK = new DBObject();
                        dbObjFK.Name = GetTableValue(row, "COLUMN_NAME");
                        var dbObj = lstDbObject.FirstOrDefault(x => x.Name == dbObjFK.Name);
                        if (dbObj == null || dbObj.Equals(default(DBObject)))
                        {
                            dbObjFK.Type = DBObjectType.Field;
                            dbObjFK.FieldInfo.IsForeignKey = true;
                            dbObjFK.FieldInfo.ReferenceTable = GetTableValue(row, "REFERENCED_TABLE_NAME");
                            dbObjFK.FieldInfo.ReferenceColumn = GetTableValue(row, "REFERENCED_COLUMN_NAME");
                            lstDbObject.Add(dbObjFK);
                        }
                        else
                        {
                            dbObj.FieldInfo.IsForeignKey = true;
                            dbObj.FieldInfo.ReferenceTable = GetTableValue(row, "REFERENCED_TABLE_NAME");
                            dbObj.FieldInfo.ReferenceColumn = GetTableValue(row, "REFERENCED_COLUMN_NAME");
                        }
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
            MySqlCommand dataCommand;
            var dataTableCollection = new List<DataTable>();
            using (var c = new MySqlConnection(dbConnectionString))
            {
                try
                {
                    await c.OpenAsync(token);
                    c.InfoMessage += new MySqlInfoMessageEventHandler(c_InfoMessage);

                    dataCommand = new MySqlCommand(queryText, c);

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
                                var colName = reader.GetName(i);
                                if (dtRet.Columns.Contains(colName))
                                {
                                    var sbMessage = new StringBuilder();
                                    sbMessage.AppendLine($"列名が重複しています[{colName}]");
                                    sbMessage.AppendLine($"クエリで列名を明示的に指定する必要があります");
                                    throw new Exception(sbMessage.ToString());
                                }
                                dtRet.Columns.Add(colName, reader.GetFieldType(i));
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

        private void c_InfoMessage(object sender, MySqlInfoMessageEventArgs e)
        {
            string errmsg = string.Empty;
            foreach (MySqlError err in e.Errors)
            {
                errmsg += "[" + err.Level + "]" + err.Message + "\n";
            }
            throw new Exception(errmsg);
        }
    }
}
