using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing3.API.Data;

namespace HotelListing3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(HotelListingDbContext context, ILogger<CountriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            _logger.LogInformation("Agarrando todos los paises");

            var countries = await _context.Countries.ToListAsync(); ;

            return Ok(countries);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                _logger.LogError("el id no coincide");

                return BadRequest();
            }

            // el PUT siempre reemplaza toda la entity
            //_context.Countries.Entry(country).State = EntityState.Modified; determina qes en Countries
            // xel tipo de la entity q se le pasa
            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}


// 27
