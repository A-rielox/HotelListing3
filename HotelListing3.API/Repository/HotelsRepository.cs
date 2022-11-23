using HotelListing3.API.Contracts;
using HotelListing3.API.Data;

namespace HotelListing3.API.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {

        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
        }
    }
}
