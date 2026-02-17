using TicTacToe.Domain.Models;

namespace TicTacToe.Application
{
    public interface IGameService
    {
        Task<GameSessionModel> CreateGameAsync(int size);
        Task<GameSessionModel> GetGameAsync(Guid id);
        Task<MoveStatus> MakeMoveAsync(Guid id, int x, int y, bool vsAi);
    }
}
