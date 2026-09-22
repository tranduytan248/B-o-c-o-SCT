using System.ComponentModel;

namespace CenIT.ReportTourism.Core.Conts
{
    public enum EnumTypeBusiness
    {
        [Description("TypeBusiness_Title_Accommodation")]
        Accommodation = 1,

        [Description("TypeBusiness_Title_Traveling")]
        Traveling = 2,

        [Description("TypeBusiness_Title_TransportTourists")]
        TransportTourists = 3,

        [Description("TypeBusiness_Title_TouristAttractions")]
        TouristAttraction = 4,

        [Description("TypeBusiness_Title_ServicesForTourists")]
        ServicesForTourists = 5
    }
}