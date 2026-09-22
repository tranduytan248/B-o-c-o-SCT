using System.Web.Mvc;
using TSFramework.Core.Enums;

namespace TSFramework.App.Attributes
{
    public class AllowAnyPermissionAttribute : FilterAttribute
    {
    }

    public class ActionTypeAttribute : FilterAttribute
    {
        public EnumActionType Type { get; set; }
    }
}