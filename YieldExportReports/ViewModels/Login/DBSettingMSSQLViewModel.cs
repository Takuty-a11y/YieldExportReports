using System;
using YieldExportReports.Database.DBLibraries;
using YieldExportReports.Database.DBResources;
using YieldExportReports.ViewModels.Base;

namespace YieldExportReports.ViewModels.Login
{
    public class DBSettingMSSQLViewModel : DBSettingViewModel
    {
        public DBSettingMSSQLViewModel() { }
        public DBSettingMSSQLViewModel(DBResource? dBResource)
        {
            if (dBResource == null) return;
            InitializeSetting(dBResource);
        }
        protected override void InitializeSetting(DBResource dBResource)
        {
            ServerText = dBResource.Name;
            DatabaseText = dBResource.DefaultDB;
            TimeoutText = dBResource.TimeOut;
            InstanceText = dBResource.InstanceName;
            if (string.IsNullOrEmpty(dBResource.IntegratedSecurity))
            {
                IsIntegrate = false;
                UserText = dBResource.UserName;
                PasswordText = dBResource.Password;
            }
            else
            {
                IsIntegrate = true;
                UserText = string.Empty;
                PasswordText = string.Empty;
            }
        }

        public override DBResource GetDataSource()
        {
            var dBResource = new DBResource();
            dBResource.ID = Guid.NewGuid();
            dBResource.DefaultDB = DatabaseText;
            if (IsIntegrate == true)
                dBResource.IntegratedSecurity = "SSPI";
            else
                dBResource.IntegratedSecurity = string.Empty;
            dBResource.IsConnected = true;
            dBResource.Name = ServerText;
            dBResource.PersistSecurityInfo = "TRUE";
            dBResource.TimeOut = TimeoutText;
            dBResource.UserName = UserText;
            dBResource.Password = PasswordText;
            dBResource.InstanceName = InstanceText;

            dBResource.DBType = DBType.MicrosoftSQLServer;

            return dBResource;
        }
    }
}
