using System;
using System.Xml.Serialization;

namespace YieldExportReports.Report.ReportFields
{
    [Serializable]
    public class ReportField
    {
        public Guid ID { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        [XmlIgnore]
        public Type? DataType { get { return Type.GetType(TypeName); } }
        public string TypeName { get; set; } = string.Empty;
    }
}
