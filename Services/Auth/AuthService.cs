using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PremiumPlace_API.Data;
using PremiumPlace_API.Models;
using PremiumPlace_API.Models.DTO.Auth;

namespace PremiumPlace_API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            ApplicationDbContext db,
            ITokenService tokenService,
            IPasswordService passwordService,
            IOptions<JwtOptions> jwtOptions)
        {
            _db = db;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _jwtOptions = jwtOptions.Value;
        }
        public async Task<ServiceResponse<AuthResultDTO>> LoginAsync(LoginRequestDTO dto, string ip, string? userAgent)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> LogoutAsync(string refreshToken, string ip)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<AuthResultDTO>> RefreshAsync(string refreshToken, string ip, string? userAgent)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<AuthResultDTO>> RegisterAsync(RegisterRequestDTO dto, string ip, string? userAgent)
        {
            var email = dto.Email.Trim().ToLower();
            var username = dto.Username.Trim();

            var existingUserByEmail = await _db.Users
                .AnyAsync(u => u.Email == email || u.Username == username);

            if (existingUserByEmail) {
                return Fail<AuthResultDTO>(
                    ServiceErrorType.Conflict,
                    "User with same email or username already exists.");
            }

            var user = new User
            {
                Email= email,
                Username = username,
                Role = UserRole.User,
            };
            user.PasswordHash = _passwordService.Hash(user, dto.Password);

            var refreshPlain = _tokenService.CreateRefreshToken();
            var refreshToken = _tokenService.HashToken(refreshPlain);

            user.RefreshTokens.Add(new RefreshToken
            {
               TokenHash = refreshToken,
               ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
               CreatedByIp = ip,
            });

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            var accessToken = _tokenService.CreateAccessToken(user);

            return Ok(new AuthResultDTO
            {
                User = MapUser(user),
                AccessToken = accessToken,
                RefreshToken = refreshPlain
            });
        }

        private static AuthUserDTO MapUser(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role.ToString(),
        };
        private static ServiceResponse<T> Ok<T>(T data) => new() { Data = data, Success = true };
        private static ServiceResponse<T> Fail<T>(
           ServiceErrorType type,
           string message,
           string? error = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                ErrorType = type,
                Message = message,
                Error = error
            };
        }


    }
}
