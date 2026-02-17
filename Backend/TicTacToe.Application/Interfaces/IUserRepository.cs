using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(UserModel user);
        Task<UserModel?> GetByLoginAsync(string login);
        Task<UserModel?> GetByIdAsync(Guid id);
    }
}
