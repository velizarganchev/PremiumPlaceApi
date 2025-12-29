using PremiumPlace.DTO;
using PremiumPlace_Web.Application.Results;

namespace PremiumPlace_Web.Application.Abstractions.Api
{
    public interface IPlaceApi
    {
        Task<ApiResult<IReadOnlyList<PlaceDTO>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResult<PlaceDTO>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ApiResult<PlaceDTO>> CreateAsync(PlaceCreateDTO dto, CancellationToken ct = default);
        Task<ApiResult<PlaceDTO>> UpdateAsync(int id, PlaceUpdateDTO dto, CancellationToken ct = default);
        Task<ApiResult> DeleteAsync(int id, CancellationToken ct = default);
    }
}
