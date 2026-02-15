using System.Text.Json.Serialization;

namespace PremiumPlace.DTO.PayPal
{
    public sealed record PayPalAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
