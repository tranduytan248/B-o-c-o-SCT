using System.ComponentModel;

namespace TSFramework.Core.Enums
{
    public enum EnumProcessType
    {
        [Description("Add")] Add = 1,
        [Description("Edit")] Edit = 2,
        [Description("Delete")] Delete = 3,
        [Description("Common_DataExisted")] DataExisted = 4,
        [Description("Common_DataNotExist")] DataNotExist = 5,
        [Description("Common_NonFormat")] NonFormat = 6
    }
}