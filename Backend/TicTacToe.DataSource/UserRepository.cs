using Microsoft.EntityFrameworkCore;
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
    }
}
