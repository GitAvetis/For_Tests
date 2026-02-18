using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameSessionModel> CreateGameAsync(int size, Guid userId, bool isVsAi);
        Task<GameSessionModel> GetGameAsync(Guid id);
        Task<MoveStatus> MakeMoveAsync(Guid id, int x, int y, Guid userId);
    }
}
