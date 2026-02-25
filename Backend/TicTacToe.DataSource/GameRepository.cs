using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Interfaces;
using TicTacToe.Contracts.DTO;
using TicTacToe.DataSource.Entitys;
using TicTacToe.DataSource.Mappers;
using TicTacToe.Domain.Models;


namespace TicTacToe.DataSource
{
    public class GameRepository : IGameRepository
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

            entity.JsonField = GameEntityMapper.SerializeField(game.Field.GetFieldCopy());
            entity.CurrentPlayer = (int)game.CurrentPlayer;
            entity.Result = (int)game.Result;
            entity.Status = (int)game.Status;
            entity.PlayerOId = game.PlayerOId;

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

        public async Task<List<GameSessionModel>> GetWaitingGamesAsync()
        {
            var entities = await _dbContext.Games
                .Where(g => g.Status == (int)GameStatus.WaitingForOpponent && !g.IsVsAi).ToListAsync();
            return entities
                .Select(GameEntityMapper.ToGameSessionModel)
                .ToList();
        }

        public async Task<List<GameSessionModel>> GetFinishedGamesAsync(Guid userId)
        {
            var entities = await _dbContext.Games
            .Where(g =>
            g.Status == (int)GameStatus.Finished &&
            g.IsVsAi == false &&
            (g.PlayerOId == userId || g.PlayerXId == userId))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
            return entities
                .Select(GameEntityMapper.ToGameSessionModel)
                .ToList();
        }

        public async Task DeleteAsync(Guid id)
        {
            GameEntity entity = await _dbContext.Games.
                FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null) return;

            _dbContext.Games.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<LeaderboardDto>> GetLeaderBoardAsync(int n)
        {
            var xStats = _dbContext.Games
                .Where(g => g.Status == (int)GameStatus.Finished && !g.IsVsAi && g.PlayerXId != null)
                .Select(g => new
                {
                    UserId = g.PlayerXId.Value,
                    IsWin = g.Result == (int)GameResult.WinX ? 1 : 0,
                    IsLoss = g.Result == (int)GameResult.WinO ? 1 : 0,
                    IsDraw = g.Result == (int)GameResult.Draw ? 1 : 0
                });

            var oStats = _dbContext.Games
                .Where(g => g.Status == (int)GameStatus.Finished && !g.IsVsAi && g.PlayerOId != null)
                .Select(g => new
                {
                    UserId = g.PlayerOId.Value,
                    IsWin = g.Result == (int)GameResult.WinX ? 1 : 0,
                    IsLoss = g.Result == (int)GameResult.WinO ? 1 : 0,
                    IsDraw = g.Result == (int)GameResult.Draw ? 1 : 0
                });

            var allStats = xStats.Concat(oStats);

            var grouped = await allStats
                .GroupBy(s => s.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Wins = g.Sum(x => x.IsWin),
                    Losses = g.Sum(x => x.IsLoss),
                    Draws = g.Sum(x => x.IsDraw),
                }).ToListAsync();

            var usersId = grouped.Select(g => g.UserId).ToList();

            var usersDict = await _dbContext.Users
                .Where(u => usersId.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Login);

            var result = grouped
                .Select(g => new LeaderboardDto
                {
                    UserId = g.UserId,
                    WinRate = CalcWinRate(g.Wins, g.Losses, g.Draws),
                    Login = usersDict[g.UserId]
                })
                .OrderByDescending(w => w.WinRate)
                .Take(n)
                .ToList();
            return result;
        }

        private double CalcWinRate(int wins, int losses, int draws)
        {
            int total = wins + losses + draws;
            if (total == 0) return 0;
            return Math.Round(100.0 * wins / total, 2);
        }
    }
}
