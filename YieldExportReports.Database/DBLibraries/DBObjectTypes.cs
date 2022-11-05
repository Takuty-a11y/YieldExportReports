using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBLibraries
{
    public enum DBObjectType
    {
        Server = 0,
        DataBase = 1,
        Table = 2,
        View = 3,
        Function = 4,
        Procedure = 5,
        Field = 6,
    }

    public class DBObjectTypes
    {
        public static string GetObjectNameString(DBObjectType objectType)
        {
            switch (objectType)
            {
                case DBObjectType.Table:
                    return "テーブル";
                case DBObjectType.View:
                    return "ビュー";
                case DBObjectType.Function:
                    return "関数";
                case DBObjectType.Procedure:
                    return "ストアドプロシージャ";
                default:
                    return string.Empty;
            }
        }
        public static DBObjectType GetTypeFromString(string typeString)
        {
            return typeString switch
            {
                "U" => DBObjectType.Table,
                "V" => DBObjectType.View,
                "FN" => DBObjectType.Function,
                "P" => DBObjectType.Procedure,
                _ => DBObjectType.Server,
            };
        }
    }
}
