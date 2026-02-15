namespace PremiumPlace_API.Infrastructure.Payments.PayPal
{
    public sealed class PayPalOptions
    {
        public string Mode { get; set; } = "Sandbox"; // Sandbox | Live
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public bool CaptureOnConfirm { get; set; } = true;
        public string? ExpectedCurrency { get; set; } = "EUR";

        public Uri BaseUri => string.Equals(Mode, "Live", StringComparison.OrdinalIgnoreCase)
            ? new Uri("https://api-m.paypal.com")
            : new Uri("https://api-m.sandbox.paypal.com");
    }
}
