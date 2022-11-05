using System;
using System.Windows.Data;

namespace YieldExportReports.Dock
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ContentPropertyAttribute : Attribute
    {
        public BindingMode BindingMode { get; set; }

        public ContentPropertyAttribute()
        { }
    }
}
