using System.Text.Json.Serialization;

namespace PremiumPlace.DTO.PayPal
{
    public sealed record PayPalOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = ""; // CREATED / APPROVED / COMPLETED ...

        [JsonPropertyName("purchase_units")]
        public List<PayPalPurchaseUnit> PurchaseUnits { get; set; } = new();
    }

    public sealed record PayPalPurchaseUnit
    {
        [JsonPropertyName("amount")]
        public PayPalAmount Amount { get; set; } = new();

        [JsonPropertyName("payments")]
        public PayPalPayments? Payments { get; set; }
    }

    public sealed record PayPalPayments
    {
        [JsonPropertyName("captures")]
        public List<PayPalCapture> Captures { get; set; } = new();
    }

    public sealed record PayPalCapture
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = ""; // COMPLETED

        [JsonPropertyName("amount")]
        public PayPalAmount Amount { get; set; } = new();
    }

    public sealed record PayPalAmount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = "";

        [JsonPropertyName("value")]
        public string Value { get; set; } = ""; // "123.45"
    }
}
