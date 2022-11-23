using AutoMapper;
using HotelListing3.API.Data;
using HotelListing3.API.Models.Country;
using HotelListing3.API.Models.Hotels;

namespace HotelListing3.API.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();

            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();

            //CreateMap<ApiUser, ApiUserDto>().ReverseMap();
        }
    }
}
