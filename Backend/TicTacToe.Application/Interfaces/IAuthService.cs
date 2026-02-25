using TicTacToe.Contracts.DTO;

namespace TicTacToe.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(SignUpRequest request);
        Task<JwtResponse> LoginAsync(JwtRequest request);
        Task<JwtResponse> RefreshAccessTokenAsync(string refreshToken);
        Task<JwtResponse> RefreshRefreshTokenAsync(string refreshToken);
    }
}
