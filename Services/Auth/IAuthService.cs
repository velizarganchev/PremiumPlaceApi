using PremiumPlace_API.Models.DTO.Auth;

namespace PremiumPlace_API.Services.Auth

{
    public interface IAuthService
    {
        Task<ServiceResponse<AuthResultDTO>> RegisterAsync(RegisterRequestDTO dto, string ip, string? userAgent);
        Task<ServiceResponse<AuthResultDTO>> LoginAsync(LoginRequestDTO dto, string ip, string? userAgent);

        // refresh token идва от cookie, controller го подава като string
        Task<ServiceResponse<AuthResultDTO>> RefreshAsync(string refreshToken, string ip, string? userAgent);

        // logout: ревокира текущия refresh token (ако има)
        Task<ServiceResponse<bool>> LogoutAsync(string refreshToken, string ip);
    }
}
