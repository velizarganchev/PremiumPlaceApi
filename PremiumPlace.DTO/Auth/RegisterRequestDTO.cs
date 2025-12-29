using System.ComponentModel.DataAnnotations;

namespace PremiumPlace.DTO.Auth
{
    public record class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Username is required!"), MinLength(3), MaxLength(80)]
        public string Username { get; init; } = default!;

        [Required(ErrorMessage = "Email is required!") , EmailAddress, MaxLength(256)]
        public string Email { get; init; } = default!;

        [Required(ErrorMessage = "Password is required!") , MinLength(8), MaxLength(128)]
        public string Password { get; init; } = default!;
    }
}
