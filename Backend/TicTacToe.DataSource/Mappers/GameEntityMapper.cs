using System.Text.Json;
using TicTacToe.DataSource.Entitys;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Mappers
{
    internal static class GameEntityMapper
    {
        public static GameEntity ToGameEntity(GameSessionModel game)
        {
            return new GameEntity
            {
                Id = game.Id,
                JsonField = JsonSerializer.Serialize(game.Field.GetFieldCopy()),
                CurrentPlayer = (int)game.CurrentPlayer,
                Result = (int)game.Result,
                CreatedAt = DateTime.UtcNow,
                IsVsAi = game.IsVsAi
            };
        }

        public static GameSessionModel ToGameSessionModel(GameEntity entity)
        {
            CellState[,] field = JsonSerializer.Deserialize<CellState[,]>(entity.JsonField);
            CellState currentPlayer = (CellState)entity.CurrentPlayer;
            GameResult result = (GameResult)entity.Result;
            return GameSessionModel.Restore(entity.Id, field, currentPlayer, result);
        }
    }
}
