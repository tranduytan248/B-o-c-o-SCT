using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateNationalModel
    {
        public int NationalId { get; set; }

        [CustomRequired]
        [CustomDisplayName("National_Label_NationalCode")]
        public string NationalCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("National_Label_NationalName")]
        public string NationalName { get; set; }

        [CustomRequired]
        [CustomDisplayName("National_Label_ContinentName")]
        public int? ContinentId { get; set; }

        [CustomDisplayName("National_Label_ContinentName")]
        public string ContinentName { get; set; }

        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("National_Label_ContinentName")]
        public List<ListItem> ListContinent { get; set; }
    }
}