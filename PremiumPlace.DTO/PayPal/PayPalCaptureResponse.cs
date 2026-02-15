using System.Text.Json.Serialization;

namespace PremiumPlace.DTO.PayPal
{
    public sealed record PayPalCaptureResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = ""; // COMPLETED

        [JsonPropertyName("purchase_units")]
        public List<PayPalPurchaseUnit> PurchaseUnits { get; set; } = new();
    }
}
