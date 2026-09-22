using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysConfigsModel
    {
        public int ConfigId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Sys_Configs_ConfigValue")]
        public string ConfigValue { get; set; }

        [CustomRequired]
        [CustomDisplayName("Sys_Configs_ConfigKey")]
        public string ConfigKey { get; set; }

        [CustomDisplayName("Sys_Configs_ConfigDesc")]
        public string ConfigDesc { get; set; }

        public int? TotalRow { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }

        public string SaveBy { get; set; }

        public string DeletedBy { get; set; }
    }
}