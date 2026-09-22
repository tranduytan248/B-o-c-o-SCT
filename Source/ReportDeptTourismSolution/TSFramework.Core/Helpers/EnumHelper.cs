using System;
using System.ComponentModel;

namespace TSFramework.Core.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        ///     Retrieve the description on the enum, e.g.
        ///     [Description("Bright Pink")]
        ///     BrightPink = 2,
        ///     Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            var type = en.GetType();

            var memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0) return en.ToString();
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute) attrs[0]).Description : en.ToString();
        }
    }
}