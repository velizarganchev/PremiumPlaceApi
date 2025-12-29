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

        public async Task<ApiResult<PlaceDTO>> CreateAsync(PlaceCreateDTO dto, CancellationToken ct = default)
        {
            var resp = await _client.PostJsonAsync("/api/places", dto, ct);
            _client.ForwardSetCookieToBrowser(resp);

            if (resp.IsSuccessStatusCode)
            {
                var data = await _client.ReadJsonAsync<PlaceDTO>(resp, ct);
                return ApiResult<PlaceDTO>.Ok(data, (int)resp.StatusCode);
            }

            var (msg, errors) = await ApiErrorParser.ParseAsync(resp, ct);
            return ApiResult<PlaceDTO>.Fail((int)resp.StatusCode, msg, errors);
        }

        public async Task<ApiResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            var resp = await _client.DeleteJsonAsync($"/api/places/{id}", new { }, ct);
            _client.ForwardSetCookieToBrowser(resp);

            if (resp.IsSuccessStatusCode)
                return ApiResult.Ok((int)resp.StatusCode);

            var (msg, errors) = await ApiErrorParser.ParseAsync(resp, ct);
            return ApiResult.Fail((int)resp.StatusCode, msg, errors);
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

        public async Task<ApiResult<PlaceDTO>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var resp = await _client.GetAsync($"/api/places/{id}", ct);

            if (resp.IsSuccessStatusCode)
            {
                var place = await _client.ReadJsonAsync<PlaceDTO>(resp, ct);
                return ApiResult<PlaceDTO>.Ok(place, (int)resp.StatusCode);
            }

            var (msg, errors) = await ApiErrorParser.ParseAsync(resp, ct);
            return ApiResult<PlaceDTO>.Fail((int)resp.StatusCode, msg, errors);
        }

        public async Task<ApiResult<PlaceDTO>> UpdateAsync(int id, PlaceUpdateDTO dto, CancellationToken ct = default)
        {
            var resp = await _client.PutJsonAsync($"/api/places/{id}", dto, ct);
            _client.ForwardSetCookieToBrowser(resp);

            if (resp.IsSuccessStatusCode)
            {
                var data = await _client.ReadJsonAsync<PlaceDTO>(resp, ct);
                return ApiResult<PlaceDTO>.Ok(data, (int)resp.StatusCode);
            }

            var (msg, errors) = await ApiErrorParser.ParseAsync(resp, ct);
            return ApiResult<PlaceDTO>.Fail((int)resp.StatusCode, msg, errors);
        }
    }
}
