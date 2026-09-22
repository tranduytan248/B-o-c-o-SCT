using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateDistrictModel
    {
        public CateDistrictModel()
        {
            Provinces = new List<ListItem>();
        }

        [CustomDisplayName("District_Label_Province")]
        public int? DistrictId { get; set; }


        [CustomRequired]
        [CustomDisplayName("District_Label_Province")]

        public int ProvinceId { get; set; }

        [CustomDisplayName("District_Label_Province")]
        public string ProvinceCode { get; set; }

        [CustomDisplayName("District_Label_Province")]
        public string ProvinceName { get; set; }


        [CustomDisplayName("District_Label_Code")]
        public string DistrictCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("District_Label_Name")]
        public string DistrictName { get; set; }

        public bool IsDeleted { get; set; } = false;
        public int? TotalRow { get; set; } = 0;

        public string UserCreated { get; set; }
        public DateTime DateCreated { get; set; }


        [CustomDisplayName("District_Label_Province")]

        public List<ListItem> Provinces { get; set; }
    }
}