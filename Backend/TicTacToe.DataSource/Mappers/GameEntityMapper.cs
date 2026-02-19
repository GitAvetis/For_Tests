using System.Text.Json;
using TicTacToe.DataSource.Entitys;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Mappers
{
    internal static class GameEntityMapper
    {
        public static string SerializeField(CellState[,] field)
        {
            int size = field.GetLength(0);
            int[][] jsonField = new int[size][];
            for (int i = 0; i < size; i++)
            {
                jsonField[i] = new int[size];
                for (int j = 0; j < size; j++)
                {
                    jsonField[i][j] = (int)field[i, j];
                }
            }
            return JsonSerializer.Serialize(jsonField);
        }

        public static CellState[,] DeserializeField(string json)
        {
            var jsonField = JsonSerializer.Deserialize<int[][]>(json);
            int size = jsonField.Length;
            CellState[,] field = new CellState[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    field[i, j] = (CellState)jsonField[i][j];
                }
            }
            return field;
        }

        public static GameEntity ToGameEntity(GameSessionModel game)
        {
            return new GameEntity
            {
                Id = game.Id,
                JsonField = SerializeField(game.Field.GetFieldCopy()),
                CurrentPlayer = (int)game.CurrentPlayer,
                Result = (int)game.Result,
                CreatedAt = DateTime.UtcNow,
                IsVsAi = game.IsVsAi,
                Status = (int)game.Status,
                PlayerXId = game.PlayerXId,
                PlayerOId = game.PlayerOId
            };
        }

        public static GameSessionModel ToGameSessionModel(GameEntity entity)
        {
            CellState[,] field = DeserializeField(entity.JsonField);
            CellState currentPlayer = (CellState)entity.CurrentPlayer;
            GameResult result = (GameResult)entity.Result;
            GameStatus status = (GameStatus)entity.Status;
            return GameSessionModel.Restore(
                entity.Id, 
                field, 
                currentPlayer, 
                result, 
                status, 
                entity.PlayerXId,
                entity.PlayerOId,
                entity.IsVsAi);
        }
    }
}
