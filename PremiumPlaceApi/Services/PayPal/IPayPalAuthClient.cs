namespace PremiumPlace_API.Services.PayPal
{
    public interface IPayPalAuthClient
    {
        Task<string> GetAccessTokenAsync(CancellationToken ct = default);
    }
}
