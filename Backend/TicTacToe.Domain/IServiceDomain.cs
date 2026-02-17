using TicTacToe.Domain.Models;

namespace TicTacToe.Domain
{
    public interface IServiceDomain
    {
        (int x, int y) GetNextMove(GameSessionModel game);
        bool ValidateField(GameSessionModel game);
        bool IsGameOver(GameSessionModel game);
    }
}
