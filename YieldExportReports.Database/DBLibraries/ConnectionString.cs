using YieldExportReports.Database.DBConnects;
using YieldExportReports.Database.DBResources;

namespace YieldExportReports.Database.DBLibraries
{
    public static class ConnectionString
    {
        private struct StrParam
        {
            public string Password;
            public string Name;
            public DBType DBType;
            public string DefaultDB;
            public string PersistSecurityInfo;
            public string TimeOut;
            public string UserName;
            public string IntegratedSecurity;
        }
        public static string Get(DBResource dBResource)
        {
            var param = new StrParam
            {
                DBType = dBResource.DBType,
                Name = dBResource.Name,
                PersistSecurityInfo = dBResource.PersistSecurityInfo,
                DefaultDB = dBResource.DefaultDB,
                UserName = dBResource.UserName,
                Password = dBResource.Password,
                TimeOut = dBResource.TimeOut,
                IntegratedSecurity = dBResource.IntegratedSecurity
            };

            return ConnectionStringByDBType(param);
        }
        public static string Get(DBConnect dBConnect)
        {
            var param = new StrParam
            {
                DBType = dBConnect.DBType,
                Name = dBConnect.ConnectionName,
                PersistSecurityInfo = dBConnect.PersistSecurityInfo,
                DefaultDB = dBConnect.DefaultDB,
                UserName = dBConnect.UserName,
                Password = dBConnect.Password,
                TimeOut = dBConnect.TimeOut,
                IntegratedSecurity = dBConnect.IntegratedSecurity
            };

            return ConnectionStringByDBType(param);
        }
        private static string ConnectionStringByDBType(StrParam param)
        {
            switch (param.DBType)
            {
                case DBType.MicrosoftSQLServer:
                    return MicrosoftSqlClientConnectionString(param);
                case DBType.MicrosoftOleDb:
                    return MicrosoftOleDbConnectionString(param);
                case DBType.PostgreSQL:
                    return NpgsqlConnectionString(param);
                case DBType.MySQL:
                    return MySQLClientConnectionString(param);
                default:
                    return MicrosoftSqlClientConnectionString(param);
            }
        }
        private static string MicrosoftSqlClientConnectionString(StrParam param)
        {
            var sConStr =
                $"Data Source={param.Name}; " +
                $"Persist Security Info={param.PersistSecurityInfo}; " +
                $"Initial Catalog={param.DefaultDB}; " +
                $"User ID={param.UserName}; " +
                $"Password={param.Password}; " +
                $"Connection Timeout={param.TimeOut}";

            if (string.IsNullOrEmpty(param.IntegratedSecurity))
                sConStr += $"; Integrated Security={param.IntegratedSecurity}";

            return sConStr;
        }
        private static string MicrosoftOleDbConnectionString(StrParam param)
        {
            if (string.IsNullOrEmpty(param.IntegratedSecurity))
            {
                return
                    $"Provider=sqloledb;" +
                    $"Data Source={param.Name};" +
                    $"Initial Catalog={param.DefaultDB};" +
                    $"User Id={param.UserName};" +
                    $"Password={param.Password};";
            }
            else
            {
                return
                    $"Provider=sqloledb;" +
                    $"Data Source={param.Name};" +
                    $"Initial Catalog={param.DefaultDB};" +
                    $";" +
                    $"Integrated Security={param.IntegratedSecurity};";
            }
        }
        private static string NpgsqlConnectionString(StrParam param)
        {
            if (string.IsNullOrEmpty(param.IntegratedSecurity))
            {
                return
                    $"Server={param.Name};" +
                    $"Database={param.DefaultDB}; " +
                    $"User Id={param.UserName};" +
                    $"Password={param.Password}";
            }
            else
            {
                return
                    $"Server={param.Name};" +
                    $"Database={param.DefaultDB}; " +
                    $"Integrated Security={param.IntegratedSecurity}";
            }
        }
        private static string MySQLClientConnectionString(StrParam param)
        {
            return
                $"Data Source={param.Name};" +
                $"user id={param.UserName}; " +
                $"password={param.Password}; " +
                $"database={param.DefaultDB};";
        }
    }
}
