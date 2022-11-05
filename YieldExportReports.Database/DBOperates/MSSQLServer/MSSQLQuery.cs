using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBOperates.MSSQLServer
{
    public static class MSSQLQuery
    {
        const string QueryString_GetTablePrimaryKey =
                        @"SELECT
                             tbls.name AS table_name
                            ,key_const.name
                            ,idx_cols.key_ordinal
                            ,cols.name AS col_name
                        FROM
                            sys.tables AS tbls
                        INNER JOIN sys.key_constraints AS key_const
                            ON tbls.object_id = key_const.parent_object_id
                            AND key_const.type = 'PK'
                            AND tbls.name = '{0}'
                        INNER JOIN sys.index_columns AS idx_cols
                            ON key_const.parent_object_id = idx_cols.object_id
                            AND key_const.unique_index_id  = idx_cols.index_id
                        INNER JOIN sys.columns AS cols
                            ON idx_cols.object_id = cols.object_id
                            AND idx_cols.column_id = cols.column_id";

        const string QueryString_GetTableForeignKey =
                        @"SELECT 
                             fk.parent_object_id AS 'ObjectId'
                            ,fk.name AS 'name' 
                            ,OBJECT_NAME(fk.parent_object_id) AS 'table_name' 
                            ,COL_NAME(fc.parent_object_id, fc.parent_column_id) AS 'column_name' 
                            ,OBJECT_NAME (fk.referenced_object_id) AS 'reference_table_name' 
                            ,COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS 'reference_column_name'
                            ,fk.is_disabled 
                        FROM
                            sys.foreign_keys AS fk 
                        INNER JOIN sys.foreign_key_columns AS fc   
                            ON fk.object_id = fc.constraint_object_id   
                        WHERE fk.parent_object_id = OBJECT_ID('{0}') ";

        public static string TablePrimaryKeyQuery(string tblName)
        {
            return string.Format(QueryString_GetTablePrimaryKey, tblName);
        }
        public static string TableForeignKeyQuery(string tblName)
        {
            return string.Format(QueryString_GetTableForeignKey, tblName);
        }
    }
}
