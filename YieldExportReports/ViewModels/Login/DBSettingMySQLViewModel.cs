using System;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBResources;
using YieldExportReports.ViewModels.Base;

namespace YieldExportReports.ViewModels.Login
{
    public class DBSettingMySQLViewModel : DBSettingViewModel
    {
        public DBSettingMySQLViewModel() { }
        public DBSettingMySQLViewModel(DBResource? dBResource)
        {
            if (dBResource == null) return;
            InitializeSetting(dBResource);
        }
        protected override void InitializeSetting(DBResource dBResource)
        {
            ServerText = dBResource.Name;
            DatabaseText = dBResource.DefaultDB;
            InstanceText = dBResource.InstanceName;
            UserText = dBResource.UserName;
            PasswordText = dBResource.Password;
        }

        public override DBResource GetDataSource()
        {
            var dBResource = new DBResource();
            dBResource.ID = Guid.NewGuid();
            dBResource.DefaultDB = DatabaseText;
            dBResource.IntegratedSecurity = string.Empty;
            dBResource.IsConnected = true;
            dBResource.Name = ServerText;
            dBResource.PersistSecurityInfo = string.Empty;
            dBResource.TimeOut = string.Empty;
            dBResource.UserName = UserText;
            dBResource.Password = PasswordText;
            dBResource.InstanceName = InstanceText;

            dBResource.DBType = DBType.MySQL;

            return dBResource;
        }

    }
}
