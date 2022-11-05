using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBOperates.PostgreSQL
{
    public static class PostgreSQLQuery
    {
        const string QueryString_GetTablePrimaryKey =
                        @"SELECT
                            ccu.column_name as COLUMN_NAME
                        FROM
	                        information_schema.table_constraints tc,
	                        information_schema.constraint_column_usage ccu
                        WHERE
	                        tc.table_catalog='{0}'
                            and
                            tc.table_schema='public'
	                        and
	                        tc.table_name='{1}'
	                        and
	                        tc.constraint_type IN ('PRIMARY KEY', 'FOREIGN KEY')
	                        and
	                        tc.table_catalog=ccu.table_catalog
	                        and
	                        tc.table_schema=ccu.table_schema
	                        and
	                        tc.table_name=ccu.table_name
	                        and
	                        tc.constraint_name=ccu.constraint_name";

        const string QueryString_GetTableForeignKey =
                        @"SELECT
                            k.table_name as TABLE_NAME,
                            k.column_name as COLUMN_NAME,
                            c.table_name as REFERENCE_TABLE_NAME,
                            c.column_name as REFERENCE_COLUMN_NAME
                        FROM
                            information_schema.table_constraints as t,
                            information_schema.key_column_usage as k,
                            information_schema.constraint_column_usage as c
                        WHERE
                            t.table_catalog='{0}'
                            AND
                            t.table_schema='public'
                            AND
                            t.table_name='{1}'
                            AND
                            t.constraint_type = 'FOREIGN KEY'
                            AND
                            t.constraint_name = k.constraint_name
                            AND
                            t.constraint_name = c.constraint_name";

        public static string TablePrimaryKey(string svrName, string tblName)
        {
            return string.Format(QueryString_GetTablePrimaryKey, svrName, tblName);
        }
        public static string TableForeignKey(string svrName, string tblName)
        {
            return string.Format(QueryString_GetTableForeignKey, svrName, tblName);
        }
    }
}
