using System.ComponentModel;

namespace TSFramework.Core.Enums
{
    public enum EnumStatus
    {
        [Description("Error")] Error = 0,
        [Description("Success")] Success = 1,
        [Description("AccessDenied")] AccessDenied = 2,
        [Description("AuthenticationDenied")] AuthenticationDenied = 3
    }
}