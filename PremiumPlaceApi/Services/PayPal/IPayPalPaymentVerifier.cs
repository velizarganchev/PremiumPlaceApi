namespace PremiumPlace_API.Services.PayPal
{
    public interface IPayPalPaymentVerifier
    {
        Task VerifyOrThrowAsync(
        string payPalOrderId,
        decimal expectedAmount,
        string expectedCurrency,
        string idempotencyKey,
        CancellationToken ct = default);
    }
}
