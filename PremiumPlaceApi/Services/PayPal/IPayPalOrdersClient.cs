using PremiumPlace.DTO.PayPal;

namespace PremiumPlace_API.Services.PayPal
{
    public interface IPayPalOrdersClient
    {
        Task<PayPalOrderResponse> GetOrderAsync(string orderId, CancellationToken ct = default);
        Task<PayPalCaptureResponse> CaptureOrderAsync(string orderId, string requestId, CancellationToken ct = default);
    }
}
