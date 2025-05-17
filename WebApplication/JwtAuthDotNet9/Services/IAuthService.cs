using JwtAuthDotNet9.Dto;
using WebAPI.DTO;
using WebAPI.Entities;

namespace JwtAuthDotNet9.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDTO request);
        Task<TokenResponseDto?> LoginAsync(UserDTO request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
