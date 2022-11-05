using System;
using System.IO;
using YieldExportReports.Database.DBLibraries;

namespace YieldExportReports.Database.DBResources
{
    [Serializable]
    public class DBResource
    {
        public Guid ID { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool IsEncrypt { get; set; }
        public string Name { get; set; } = string.Empty;
        public DBType DBType { get; set; }
        public string DefaultDB { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
        public string PersistSecurityInfo { get; set; } = string.Empty;
        public string TimeOut { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public string IntegratedSecurity { get; set; } = string.Empty;
        public string SettingFilePath { get; set; } = string.Empty;
        public string SettingFileName
        {
            get
            {
                try { return Path.GetFileName(SettingFilePath); }
                catch { return string.Empty; }
            }
        }
        public string DBTypeString
        {
            get
            {
                return DBType switch
                {
                    DBType.MicrosoftSQLServer => DBTypes.GetName(DBType.MicrosoftSQLServer),
                    DBType.MicrosoftOleDb => DBTypes.GetName(DBType.MicrosoftOleDb),
                    DBType.PostgreSQL => DBTypes.GetName(DBType.PostgreSQL),
                    DBType.MySQL => DBTypes.GetName(DBType.MySQL),
                    _ => string.Empty,
                };
            }
        }
        public string ConnectionString => DBLibraries.ConnectionString.Get(this);
    }
}
