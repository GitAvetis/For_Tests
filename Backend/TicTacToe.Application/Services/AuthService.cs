using TicTacToe.Application.Interfaces;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<Guid?> AuthenticateAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                return null;
            }

            if (user.ValidatePassword(password))
            {
                return user.Id;
            }
            else
            {
                return null;
            }
        }

    }
}
