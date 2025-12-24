using System.ComponentModel.DataAnnotations;

namespace PremiumPlace_API.Models.DTO.Auth
{
    public record class LoginRequestDTO
    {
        [Required]
        public string UsernameOrEmail { get; init; } = default!;

        [Required]
        public string Password { get; init; } = default!;
    }
}
