using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing3.API.Data;
using HotelListing3.API.Models.Country;
using AutoMapper;
using HotelListing3.API.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;
        private readonly IMapper _mapper;

        public CountriesController( ICountriesRepository countriesRepository,
                                    ILogger<CountriesController> logger,
                                    IMapper mapper)
        {
            _countriesRepository = countriesRepository;
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

            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<IEnumerable<GetCountryDto>>(countries);

            return Ok(records);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Countries/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);

            if (country == null) return NotFound();

            var result = _mapper.Map<CountryDto>(country);

            return result;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // PUT: api/Countries/5
        [HttpPut("{id}")]
        [Authorize(Roles="Administrator")] // si tengo token p' no soy admin 403-Forbidden
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                _logger.LogError("el id no coincide");

                return BadRequest();
            }

            var country = await _countriesRepository.GetAsync(id);

            if (country == null) return NotFound();

            // usa la data de updatedCountryDto para editar country, y ya 
            _mapper.Map(updateCountryDto, country);

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
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

            var result = await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = result.Id }, result);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null) return NotFound();

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exist(id);
        }
    }
}
