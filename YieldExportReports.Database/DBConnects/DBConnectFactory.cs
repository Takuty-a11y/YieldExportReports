using MySqlConnector;
using Npgsql;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBOperates;

namespace YieldExportReports.Database.DBConnects
{
    public static class DBConnectFactory
    {
        public static IDbConnection GetConnection(DBType connectionType, string connectionString)
        {
            return connectionType switch
            {
                DBType.MicrosoftSQLServer => new SqlConnection(connectionString),
                DBType.MicrosoftOleDb => new OleDbConnection(connectionString),
                DBType.PostgreSQL => new NpgsqlConnection(connectionString),
                DBType.MySQL => new MySqlConnection(connectionString),
                _ => new SqlConnection(connectionString),
            };
        }
        public static ISQLOperator CreateOperator(DBType connectionType)
        {
            return connectionType switch
            {
                DBType.MicrosoftSQLServer => new DBOperates.MSSQLServer.SQLOperator(),
                DBType.MicrosoftOleDb => new DBOperates.MSOLEDB.SQLOperator(),
                DBType.PostgreSQL => new DBOperates.Postgresql.SQLOperator(),
                DBType.MySQL => new DBOperates.MySQL.SQLOperator(),
                _ => new DBOperates.MSSQLServer.SQLOperator(),
            };
        }
    }
}
