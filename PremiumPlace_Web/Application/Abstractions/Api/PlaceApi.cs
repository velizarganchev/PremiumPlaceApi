using PremiumPlace.DTO;
using PremiumPlace_Web.Application.Results;
using PremiumPlace_Web.Infrastructure.Http;

namespace PremiumPlace_Web.Application.Abstractions.Api
{
    public class PlaceApi : IPlaceApi
    {
        private readonly PremiumPlaceApiClient _client;
        public PlaceApi(PremiumPlaceApiClient client) 
        {
            _client = client;
        }

        public Task<ApiResult<PlaceDTO>> CreateAsync(PlaceCreateDTO dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<IReadOnlyList<PlaceDTO>>> GetAllAsync(CancellationToken ct = default)
        {
            var resp = await  _client.GetAsync("/api/places", ct);

            if (resp.IsSuccessStatusCode)
            {
                var places = await _client.ReadJsonAsync<IReadOnlyList<PlaceDTO>>(resp, ct);
                return ApiResult<IReadOnlyList<PlaceDTO>>.Ok(places, (int)resp.StatusCode);
            }

            var (msg, errors) = await ApiErrorParser.ParseAsync(resp, ct);
            return ApiResult<IReadOnlyList<PlaceDTO>>.Fail((int)resp.StatusCode, msg, errors);
        }

        public Task<ApiResult<PlaceDTO>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<PlaceDTO>> UpdateAsync(int id, PlaceUpdateDTO dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
