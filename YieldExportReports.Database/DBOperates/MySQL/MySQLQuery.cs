using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBOperates.MySQL
{
    public static class MySQLQuery
    {
        const string QueryString_GetTableKey =
                        @"SELECT
                            *
                        FROM
                            information_schema.key_column_usage
                        WHERE
                            constraint_schema='{0}'
                        AND
                            table_name='{1}'";

        public static string TableKeyQuery(string svrName, string tblName)
        {
            return string.Format(QueryString_GetTableKey, svrName, tblName);
        }
    }
}
