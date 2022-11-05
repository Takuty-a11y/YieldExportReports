using MahApps.Metro.IconPacks;
using System.Collections.Generic;
using YieldExportReports.Database.DBLibraries;

namespace YieldExportReports.Database.DBObjects
{
    public class DBObject
    {
        public string Name { get; set; } = string.Empty;
        public DBObjectType Type { get; set; }
        public List<DBObject> Children { get; set; } = new List<DBObject>();
        public DBFieldInfo FieldInfo { get; set; } = new DBFieldInfo();
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public PackIconMaterialKind IconKind
        {
            get
            {
                switch (Type)
                {
                    case DBObjectType.DataBase:
                        return PackIconMaterialKind.Database;
                    case DBObjectType.Field:
                        if (FieldInfo.IsPrimaryKey)
                            return PackIconMaterialKind.KeyVariant;
                        else if (FieldInfo.IsForeignKey)
                            return PackIconMaterialKind.KeyOutline;
                        else
                            return PackIconMaterialKind.TableColumn;
                    default:
                        return PackIconMaterialKind.Table;
                }
            }
        }
        public string Text
        {
            get
            {
                var sText = Name;
                if (Type != DBObjectType.Field)
                {
                    return sText;
                }
                else
                {
                    var sField = FieldInfo.IsPrimaryKey ? ", PK" : string.Empty;
                    sField += FieldInfo.IsForeignKey ? ", FK" : string.Empty;
                    sField += $", {FieldInfo.DataType}";
                    sField += FieldInfo.IsAllowNull ? ", NULL" : ", NULL以外";
                    sField += FieldInfo.IsForeignKey ? $", {FieldInfo.ReferenceTable}.{FieldInfo.ReferenceColumn}" : string.Empty;
                    sText += $" ({sField[2..]})";
                    return sText;
                }
            }
        }

        public DBObject CreateDBObject(DBObjectType objectType)
        {
            var dbObj = new DBObject
            {
                Type = objectType,
                Name = DBObjectTypes.GetObjectNameString(objectType)
            };
            Children.Add(dbObj);
            return dbObj;
        }
    }
}
