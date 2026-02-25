using System.Security.Claims;
using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Jwt;
using TicTacToe.Contracts.DTO;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtProvider _jwtProvider;

        public AuthService(IUserRepository userRepository, JwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<bool> RegisterAsync(SignUpRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login);
            if (user != null)
            {
                return false; // User with the same login already exists
            }
            var newUser = UserModel.Create(Guid.NewGuid(), request.Login, request.Password);
            await _userRepository.AddAsync(newUser);
            return true;
        }

        public async Task<JwtResponse> LoginAsync(JwtRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid login");
            }
            else if (user.ValidatePassword(request.Password) == false)
                throw new UnauthorizedAccessException("Invalid password");

            var accessToken = _jwtProvider.GenerateAccessToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken(user);

            return new JwtResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<JwtResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var principal = _jwtProvider.GetPrincipalFromToken(refreshToken) ??
                throw new UnauthorizedAccessException("Invalid refresh token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                throw new UnauthorizedAccessException("Invalid token claims");

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId)) ??
                throw new UnauthorizedAccessException("User not found");

            var newAccessToken = _jwtProvider.GenerateAccessToken(user);

            return new JwtResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshToken
            };

        }

        public async Task<JwtResponse> RefreshRefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtProvider.GetPrincipalFromToken(refreshToken) ??
                throw new UnauthorizedAccessException("Invalid refresh token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                throw new UnauthorizedAccessException("Invalid token claims");

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId)) ??
                throw new UnauthorizedAccessException("User not found");

            var newAccessToken = _jwtProvider.GenerateAccessToken(user);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

            return new JwtResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }

    }
}
