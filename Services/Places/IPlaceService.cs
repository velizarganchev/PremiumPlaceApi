using PremiumPlace_API.Models;
using PremiumPlace_API.Models.DTO;

namespace PremiumPlace_API.Services.Places
{
    public interface IPlaceService
    {
        Task<ServiceResponse<List<Place>>> GetAllPlaces();
        Task<ServiceResponse<Place>> GetPlaceById(int id);
        Task<ServiceResponse<PlaceCreateDTO>> CreatePlace(PlaceCreateDTO placeDTO);
        Task<ServiceResponse<PlaceUpdateDTO>> UpdatePlace(int id, PlaceUpdateDTO placeDTO);
        Task<ServiceResponse<Place>> DeletePlace(int id);
    }
}
