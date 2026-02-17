using Microsoft.EntityFrameworkCore;
using TicTacToe.DataSource.Entitys;

namespace TicTacToe.DataSource
{
    public class TicTacToeDbContext : DbContext
    {
        public DbSet<GameEntity> Games => Set<GameEntity>();//переменные для таблиц из базы
        public DbSet<UserEntity> Users => Set<UserEntity>();

        public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options)
            : base(options) { }

    }
}
