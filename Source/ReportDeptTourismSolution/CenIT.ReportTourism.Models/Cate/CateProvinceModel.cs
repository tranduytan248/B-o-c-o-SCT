using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateProvinceModel
    {
        public int ProvinceId { get; set; }

        [CustomDisplayName("Province_Label_Code")]
        public string ProvinceCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Province_Label_Name")]
        public string ProvinceName { get; set; }

        public bool IsDeleted { get; set; }
        public string UserCreated { get; set; }
        public DateTime DateCreated { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}