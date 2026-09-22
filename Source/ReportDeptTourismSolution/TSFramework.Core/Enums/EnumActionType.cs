using System;
using System.ComponentModel;

namespace TSFramework.Core.Enums
{
    [Flags]
    public enum EnumActionType
    {
        [Description("View")] View = 1,
        [Description("Add")] Add = 2,
        [Description("Edit")] Edit = 4,
        [Description("Delete")] Delete = 8
    }
}