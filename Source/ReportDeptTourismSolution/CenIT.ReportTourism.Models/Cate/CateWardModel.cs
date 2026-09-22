using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateWardModel
    {
        public int WardId { get; set; }

        [CustomDisplayName("Ward_Label_Code")] public string WardCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Ward_Label_Name")]
        public string WardName { get; set; }

        public string ProvinceCode { get; set; }
        public bool IsDeleted { get; set; }
        public int? TotalRow { get; set; } = 0;

        public string UserCreated { get; set; }
        public DateTime DateCreated { get; set; }

        public bool IsSelected { get; set; }

        [CustomRequired]
        [CustomDisplayName("Province_Title")]
        public int? ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        public List<ListItem> Provinces { get; set; } = new List<ListItem>();
    }
}