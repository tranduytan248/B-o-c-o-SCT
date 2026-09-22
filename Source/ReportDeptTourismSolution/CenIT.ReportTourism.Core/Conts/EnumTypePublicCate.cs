using System.ComponentModel;

namespace CenIT.ReportTourism.Core.Conts
{
    public enum EnumTypePublicCate
    {
        [Description("AccommodationInfrastructure_Label_TypeRoom")]
        TypeRoomAccommodation = 100,

        [Description("Accommodation_Label_TypeService")]
        TypeServiceAccommodation = 200,

        [Description("TouristAttraction_Label_TypeTourismActivity")]
        TypeTourismActivity = 300,

        [Description("ServicesForTourists_Label_TypeServicesForTourist")]
        TypeServicesForTourist = 400,

        [Description("AccommodationTypeService_TypeClass")]
        TypeAccommodationClass = 500
    }
}