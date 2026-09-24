using System.ComponentModel;

namespace CenIT.ReportTourism.Core.Conts
{
    public enum EnumTypeBusiness
    {
        /// <summary>
        /// Doanh nghiệp sản xuất, kinh doanh
        /// </summary>
        [Description("TypeBusiness_Manufacturing")]
        Manufacturing = 1,

        /// <summary>
        /// Doanh nghiệp thương mại, dịch vụ
        /// </summary>
        [Description("TypeBusiness_Trading")]
        Trading = 2
    }
}