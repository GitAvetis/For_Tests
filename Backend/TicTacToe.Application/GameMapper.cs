using TicTacToe.Contracts.DTO;
using TicTacToe.Domain.Models;


namespace TicTacToe.Application
{
    public class GameMapper
    {
        public static GameDto ToDto(GameSessionModel model)
        {
            var fieldCopy = model.Field.GetFieldCopy();
            int size = fieldCopy.GetLength(0);

            var board = new string[size][];

            for (int i = 0; i < size; i++)
            {
                board[i] = new string[size];

                for (int j = 0; j < size; j++)
                {

                    board[i][j] = CellStateToString(fieldCopy[i, j]);
                }
            }

            return new GameDto
            {
                Id = model.Id,
                Size = size,
                CurrentPlayer = CellStateToString(model.CurrentPlayer),
                Result = GameResultToString(model.Result),
                PrettyField = BuildPrettyField(board),
                PlayerOId = model.PlayerOId,
                PlayerXId = model.PlayerXId,
                WinnerId = GetWinner(model),
                IsVsAi = model.IsVsAi
            };
        }

        private static Guid GetWinner(GameSessionModel model)
        {
            return model.Result switch
            {
                GameResult.WinX => model.PlayerXId ?? Guid.Empty,
                GameResult.WinO => model.PlayerOId ?? Guid.Empty,
                _ => Guid.Empty
            };
        }

        private static string CellStateToString(CellState state)
        {
            return state switch
            {
                CellState.X => "X",
                CellState.O => "O",
                _ => " "
            };
        }

        private static string[] BuildPrettyField(string[][] board)
        {
            var lines = new List<string>();

            for (int i = 0; i < board.Length; i++)
            {
                lines.Add(string.Join(" | ", board[i]));
            }

            return lines.ToArray();
        }

        private static string GameResultToString(GameResult result)
        {
            return result switch
            {
                GameResult.InProgress => "In Progress",
                GameResult.Draw => "Draw",
                GameResult.WinX => "X Wins",
                GameResult.WinO => "O Wins",
                _ => "Unknown"
            };
        }
    }
}
