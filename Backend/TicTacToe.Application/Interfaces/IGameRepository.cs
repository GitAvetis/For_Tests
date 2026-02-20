using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Interfaces
{
    public interface IGameRepository
    {
        Task AddAsync(GameSessionModel game);
        Task<GameSessionModel?> GetByIdAsync(Guid id);
        Task UpdateAsync(GameSessionModel game);
        Task<List<GameSessionModel>> GetWaitingGamesAsync();
        Task<List<GameSessionModel>> GetFinishedGamesAsync(Guid userId);
        Task DeleteAsync(Guid id);
    }
}
