using PremiumPlace.DTO.PayPal;
using PremiumPlace_API.Infrastructure.Payments.PayPal;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PremiumPlace_API.Services.PayPal
{
    public class PayPalOrdersClient: IPayPalOrdersClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPayPalAuthClient _authClient;
        private readonly ILogger<PayPalOrdersClient> _logger;

        public PayPalOrdersClient(
            IHttpClientFactory httpClientFactory,
            IPayPalAuthClient authClient,
            ILogger<PayPalOrdersClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _authClient = authClient;
            _logger = logger;
        }

        public async Task<PayPalOrderResponse> GetOrderAsync(string orderId, CancellationToken ct = default)
        {
            var token = await _authClient.GetAccessTokenAsync(ct);
            var client = _httpClientFactory.CreateClient(PayPalHttpClient.HttpClientName);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/checkout/orders/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayPal GetOrder failed. Status: {Status}. Body: {Body}", response.StatusCode, body);
                throw new InvalidOperationException("PayPal GetOrder failed.");
            }

            var order = JsonSerializer.Deserialize<PayPalOrderResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return order ?? throw new InvalidOperationException("PayPal order response is invalid.");
        }

        public async Task<PayPalCaptureResponse> CaptureOrderAsync(string orderId, string requestId, CancellationToken ct = default)
        {
            var token = await _authClient.GetAccessTokenAsync(ct);
            var client = _httpClientFactory.CreateClient(PayPalHttpClient.HttpClientName);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"/v2/checkout/orders/{orderId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Idempotency (ако user натисне Confirm 2 пъти)
            request.Headers.TryAddWithoutValidation("PayPal-Request-Id", requestId);

            request.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayPal Capture failed. Status: {Status}. Body: {Body}", response.StatusCode, body);
                throw new InvalidOperationException("PayPal Capture failed.");
            }

            var capture = JsonSerializer.Deserialize<PayPalCaptureResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return capture ?? throw new InvalidOperationException("PayPal capture response is invalid.");
        }
    }
}
