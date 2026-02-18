using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicTacToe.Application.Interfaces;
using TicTacToe.DataSource.Entitys;
using TicTacToe.DataSource.Mappers;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource
{
    public class UserRepository : IUserRepository
    {

        private readonly TicTacToeDbContext _dbContext;

        public UserRepository(TicTacToeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserModel user)
        {
            UserEntity entity = UserEntityMapper.ToUserEntity(user);
            _dbContext.Users.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserModel?> GetByLoginAsync(string login)
        {
            UserEntity user = await _dbContext.Users.
                FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
                return null;

            return UserEntityMapper.ToUserModel(user);
        }

        public async Task<UserModel?> GetByIdAsync(Guid userId)
        {
            UserEntity user = await _dbContext.Users.
                FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return UserEntityMapper.ToUserModel(user);

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
        .ToList();
        }

    }
}
