using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PremiumPlace_API.Data;
using PremiumPlace_API.Models;
using PremiumPlace_API.Models.DTO;
using PremiumPlace_API.Services.Places;

namespace PremiumPlace_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        public PlacesController(IPlaceService placeService, ApplicationDbContext db, IMapper mapper) 
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            var response = await _placeService.GetAllPlaces();
            if (response.Data == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Place>> GetPlaceById(int id)
        {
            try {
                if (id <= 0)
                {
                    return BadRequest();
                }
                var response = await _placeService.GetPlaceById(id);
                if (response.Data == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception) {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlaceCreateDTO>> CreatePlace(PlaceCreateDTO placeDTO)
        {
            try
            {
                if (placeDTO == null)
                {
                    return BadRequest("Place data is required!");
                }

                var response = await _placeService.CreatePlace(placeDTO);

                return CreatedAtAction(nameof(CreatePlace), placeDTO);

            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PlaceCreateDTO>> UpdatePlace(int id, PlaceUpdateDTO placeDTO)
        {
            try
            {
                if (placeDTO == null)
                {
                    return BadRequest("Place data is required!");
                }

                if (id != placeDTO.Id)
                {
                    return BadRequest($"Id's do not match!");
                }

                var response = await _placeService.UpdatePlace(id ,placeDTO);
                if (response.Data == null)
                {
                    return NotFound($"Place with Id = {id} not found!");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Place>> DeletePlace(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }
                var response = await _placeService.DeletePlace(id);
                if (response.Success == false)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
