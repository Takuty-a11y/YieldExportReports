namespace YieldExportReports.Database.DBLibraries
{
    public enum DBType
    {
        MicrosoftSQLServer = 0,
        MicrosoftOleDb = 1,
        PostgreSQL = 2,
        MySQL = 3,
    }

    public class DBTypes
    {
        public static string GetName(DBType? enmType)
        {
            return enmType switch
            {
                DBType.MicrosoftSQLServer => "Microsoft SQLServer",
                DBType.MicrosoftOleDb => "Microsoft OleDb",
                DBType.PostgreSQL => "PostgreSQL",
                DBType.MySQL => "MySQL",
                _ => string.Empty,
            };
        }
    }
}
