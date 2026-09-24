using System;
using CenIT.ReportTourism.Models.Sys;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Models.Search
{
    public class SearchEnterpriseModel : SysSearchModel
    {
        public string ForUser { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public string TypeBusinessIds { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<int> ListTypeBusinessId { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<ListItem> ListTypeBusiness { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        public string WardIds { get; set; }
        [CustomDisplayName("Enterprise_Label_Ward")]
        public List<int> ListWardId { get; set; }
        [CustomDisplayName("Enterprise_Label_Ward")]
        public List<ListItem> ListWards { get; set; }

        [CustomDisplayName("Enterprise_Label_Province")]
        public string ProvinceIds { get; set; }
        [CustomDisplayName("Enterprise_Label_Province")]
        public List<int> ListProvinceId { get; set; }
        [CustomDisplayName("Enterprise_Label_Province")]
        public List<ListItem> ListProvinces { get; set; }

        [CustomDisplayName("Enterprise_MainIndustry")]
        public string MainIndustryIds { get; set; }
        [CustomDisplayName("Enterprise_MainIndustry")]
        public List<int> ListMainIndustryId { get; set; }
        [CustomDisplayName("Enterprise_MainIndustry")]
        public List<ListItem> ListMainIndustry { get; set; }

        [CustomDisplayName("Enterprise_EnterpriseType")]
        public string EnterpriseTypeIds { get; set; }
        [CustomDisplayName("Enterprise_EnterpriseType")]
        public List<int> ListEnterpriseTypeId { get; set; }
        [CustomDisplayName("Enterprise_EnterpriseType")]
        public List<ListItem> ListEnterpriseType { get; set; }

        [CustomDisplayName("Enterprise_EconomicSector")]
        public string EconomicSectorIds { get; set; }
        [CustomDisplayName("Enterprise_EconomicSector")]
        public List<int> ListEconomicSectorId { get; set; }
        [CustomDisplayName("Enterprise_EconomicSector")]
        public List<ListItem> ListEconomicSector { get; set; }

        [CustomDisplayName("Enterprise_Status")]
        public string StatusIds { get; set; }
        [CustomDisplayName("Enterprise_Status")]
        public List<int> ListStatusId { get; set; }
        [CustomDisplayName("Enterprise_Status")]
        public List<ListItem> ListStatus { get; set; }
    }
}