namespace PremiumPlace_API.Models.DTO.Auth
{
    public record class AuthResponseDTO
    {
        public AuthUserDTO User { get; init; } = default!;
    }
}
