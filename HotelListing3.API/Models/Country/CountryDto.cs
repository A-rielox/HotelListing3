using HotelListing3.API.Models.Hotels;

namespace HotelListing3.API.Models.Country
{
    public class CountryDto : BaseCountryDto
    {
        public int Id { get; set; }
        public List<HotelDto> Hotels { get; set; }
    }
}
