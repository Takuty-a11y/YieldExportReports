using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBObjects
{
    public class DBFieldInfo
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsAllowNull { get; set; }
        public string DataType { get; set; } = string.Empty;
        public string ReferenceTable { get; set; } = string.Empty;
        public string ReferenceColumn { get; set; } = string.Empty;

    }
}
