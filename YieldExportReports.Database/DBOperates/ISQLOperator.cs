using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using YieldExportReports.Database.DBObjects;

namespace YieldExportReports.Database.DBOperates
{
    public interface ISQLOperator
    {
        DBObjectCollection GetDBObjectsToList(IDbConnection dbConnection);
        Task<List<DataTable>> GetQueryDataToDataTables
            (string queryText, string dbConnectionString, CancellationToken token);
    }
}
