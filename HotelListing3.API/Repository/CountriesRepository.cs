using HotelListing3.API.Contracts;
using HotelListing3.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing3.API.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;

        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            _context = context;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task<Country> GetDetails(int id)
        {
            var country = await _context.Countries.
                                         Include(c => c.Hotels).
                                         FirstOrDefaultAsync(c => c.Id == id);

            return country;
        }
    }
}
