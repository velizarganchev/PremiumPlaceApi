using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PremiumPlace.DTO.PayPal;
using PremiumPlace_API.Infrastructure.Payments.PayPal;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PremiumPlace_API.Services.PayPal
{
    public class PayPalAuthClient: IPayPalAuthClient
    {
        private const string CacheKey = "paypal_oauth_access_token";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly PayPalOptions _options;
        private readonly ILogger<PayPalAuthClient> _logger;

        public PayPalAuthClient(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IOptions<PayPalOptions> options,
            ILogger<PayPalAuthClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
        {
            if (_cache.TryGetValue<string>(CacheKey, out var cached) && !string.IsNullOrWhiteSpace(cached))
                return cached;

            if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
                throw new InvalidOperationException("PayPal ClientId/ClientSecret are not configured.");

            var client = _httpClientFactory.CreateClient(PayPalHttpClient.HttpClientName);

            using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            using var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayPal token request failed. Status: {Status}. Body: {Body}", response.StatusCode, body);
                throw new InvalidOperationException("PayPal OAuth token request failed.");
            }

            var token = JsonSerializer.Deserialize<PayPalAccessTokenResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
                throw new InvalidOperationException("PayPal OAuth token response is invalid.");

            // Кеш: expires_in - 60s buffer
            var ttlSeconds = Math.Max(60, token.ExpiresIn - 60);
            _cache.Set(CacheKey, token.AccessToken, TimeSpan.FromSeconds(ttlSeconds));

            return token.AccessToken;
        }
    }
}
