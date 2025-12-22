using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PremiumPlace_API.Data;
using PremiumPlace_API.Models;
using PremiumPlace_API.Models.DTO;

namespace PremiumPlace_API.Services.Places
{
    public class PlaceService : IPlaceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public PlaceService(ApplicationDbContext db, IMapper mapper) 
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<PlaceCreateDTO>> CreatePlace(PlaceCreateDTO placeDTO)
        {
            var response = new ServiceResponse<PlaceCreateDTO>();
            var place = _mapper.Map<Place>(placeDTO);
            await _db.Places.AddAsync(place);
            await _db.SaveChangesAsync();

            response.Success = true;
            response.Data = placeDTO;
            response.Message = "Place created successfully.";
            return response;
        }

        public async Task<ServiceResponse<Place>> DeletePlace(int id)
        {
            var response = new ServiceResponse<Place>();

            var place = await _db.Places.FirstOrDefaultAsync(p => p.Id == id);
            if (place == null)
            {
                response.Success = false;
                response.Message = "Place not found.";
                return response;
            }

            _db.Places.Remove(place);
            await _db.SaveChangesAsync();

            response.Success = true;
            response.Message = "Place deleted successfully.";
            return response;
        }

        public async Task<ServiceResponse<List<Place>>> GetAllPlaces()
        {
            var serviceResponse = new ServiceResponse<List<Place>>
            {
                Success = true,
                Data = await _db.Places.ToListAsync()
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<Place>> GetPlaceById(int id)
        {
            var serviceResponse = new ServiceResponse<Place>
            {
                Success = true,
                Data = await _db.Places.FirstOrDefaultAsync(p => p.Id == id)
            };
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<PlaceUpdateDTO>> UpdatePlace(int id,PlaceUpdateDTO placeDTO)
        {
            var response = new ServiceResponse<PlaceUpdateDTO>();
            var placeInDb = await _db.Places.FirstOrDefaultAsync(p => p.Id == id);
            if (placeInDb == null)
            {
                response.Success = false;
                response.Message = "Place not found.";
                return response;
            }
            _mapper.Map(placeDTO, placeInDb);
            placeInDb.UpdatedDate = DateTime.Now;
            await _db.SaveChangesAsync();

            response.Success = true;
            response.Data = placeDTO;
            response.Message = "Place updated successfully.";

            return response;
        }
    }
}
