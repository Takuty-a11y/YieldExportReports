using System.Data;
using System.Data.SqlClient;
using YieldExportReports.Database.DBLibraries;

namespace YieldExportReports.Database.DBConnects
{
    public class DBConnect
    {
        public string ConnectionString => DBLibraries.ConnectionString.Get(this);
        public string ConnectionName { get; set; } = string.Empty;
        public IDbConnection Connection { get; set; } = new SqlConnection();
        public bool IsConnected { get; set; }
        public string DefaultDB { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public DBType DBType { get; set; }
        public string PersistSecurityInfo { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TimeOut { get; set; } = string.Empty;
        public string IntegratedSecurity { get; set; } = string.Empty;
    }
}
