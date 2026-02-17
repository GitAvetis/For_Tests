using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicTacToe.Application.Interfaces;
using TicTacToe.DataSource.Entitys;
using TicTacToe.DataSource.Mappers;
using TicTacToe.Domain.Models;


namespace TicTacToe.DataSource
{
    internal class GameRepository : IGameRepository
    {
        private readonly TicTacToeDbContext _dbContext;

        public GameRepository(TicTacToeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(GameSessionModel game)
        {
            GameEntity entity = GameEntityMapper.ToGameEntity(game);
            _dbContext.Games.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GameSessionModel?> GetByIdAsync(Guid id)
        {
            GameEntity entity = await _dbContext.Games.
                FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null)
                return null;

            return GameEntityMapper.ToGameSessionModel(entity);

        }

        public async Task UpdateAsync(GameSessionModel game)
        {
            GameEntity entity = await _dbContext.Games.
                FirstOrDefaultAsync(g => g.Id == game.Id);

            if (entity == null)
                return;

            entity.JsonField = JsonSerializer.Serialize(game.Field.GetFieldCopy());
            entity.CurrentPlayer = (int)game.CurrentPlayer;
            entity.Result = (int)game.Result;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(GameSessionModel game)
        {
            GameEntity entity = await _dbContext.Games.
                FirstOrDefaultAsync(g => g.Id == game.Id);

            if (entity == null) return;

            _dbContext.Games.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GameSessionModel>> GetAllActiveGames()
        {
            var entities = await _dbContext.Games
                .Where(g => g.Result == (int)GameResult.InProgress).ToListAsync();


            return entities
                .Select(GameEntityMapper.ToGameSessionModel)
        .       ToList();
        }
    }
}
