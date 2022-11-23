using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing3.API.Data;
using HotelListing3.API.Contracts;
using AutoMapper;
using HotelListing3.API.Models.Hotels;

namespace HotelListing3.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IMapper _mapper;

        public HotelsController(IHotelsRepository hotelsRepository, IMapper mapper)
        {
            _hotelsRepository = hotelsRepository;
            _mapper = mapper;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync();
            var records = _mapper.Map<IEnumerable<HotelDto>>(hotels);

            return Ok(records);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null) return NotFound();

           var record = _mapper.Map<HotelDto>(hotel);

            return record;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.Id)
            {
                return BadRequest();
            }

            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null) return NotFound();

            _mapper.Map(hotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
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
        // POST: api/Hotels
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
        {
            var hotel = _mapper.Map<Hotel>(hotelDto);

            var hotelAdded = await _hotelsRepository.AddAsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotelAdded.Id }, hotelAdded);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);
            // tengo q hacer este check para retornar el notFound si es qes null
            if(hotel == null) return NotFound();

            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exist(id);
        }
    }
}
