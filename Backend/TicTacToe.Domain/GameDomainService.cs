using TicTacToe.Domain.Ai;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain
{
    public class GameDomainService : IServiceDomain
    {
        private readonly MiniMax _miniMax;

        public GameDomainService()
        {
            _miniMax = new MiniMax();
        }

        public (int x, int y) GetNextMove(GameSessionModel game)
        {
            return _miniMax.GetBestMove(game.CurrentPlayer, game.Field.GetFieldCopy());
        }

        public bool ValidateField(GameSessionModel game)
        {
            var field = game.Field.GetFieldCopy();
            int xCount = 0;
            int oCount = 0;

            foreach (var cell in field)
            {
                if (cell == CellState.X) xCount++;
                if (cell == CellState.O) oCount++;
            }

            return xCount == oCount || xCount == oCount + 1;
        }

        public bool IsGameOver(GameSessionModel game)
        {
            return game.Result != GameResult.InProgress;
        }
    }
}
