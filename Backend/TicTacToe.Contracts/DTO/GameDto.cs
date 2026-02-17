using TicTacToe.Domain.Models;

namespace TicTacToe.Contracts.DTO
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public CellState CurrentPlayer { get; set; }
        public GameResult Result { get; set; }
        public string[] PrettyField { get; set; } = default!;
    }
}
