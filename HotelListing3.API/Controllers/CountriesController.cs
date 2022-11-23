﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing3.API.Data;
using HotelListing3.API.Models.Country;
using AutoMapper;

namespace HotelListing3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly ILogger<CountriesController> _logger;
        private readonly IMapper _mapper;

        public CountriesController( HotelListingDbContext context,
                                    ILogger<CountriesController> logger,
                                    IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            _logger.LogInformation("Agarrando todos los paises");

            var countries = await _context.Countries.ToListAsync();

            var records = _mapper.Map<IEnumerable<GetCountryDto>>(countries);

            return Ok(records);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _context.Countries
                                        .Include(c => c.Hotels)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<CountryDto>(country);

            return result;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                _logger.LogError("el id no coincide");

                return BadRequest();
            }

            var country = await _context.Countries.FindAsync(id);
            
            if(country == null) return NotFound();

            // usa la data de updatedCountryDto para editar country, y ya 
            _mapper.Map(updateCountryDto, country);

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
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            var country = _mapper.Map<Country>(createCountryDto);

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
