using TicTacToe.Application.Interfaces;
using TicTacToe.Domain;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IServiceDomain _domainService;

        public GameService(IGameRepository repository, IServiceDomain serviceDomain)
        {
            _repository = repository;
            _domainService = serviceDomain;
        }

        public async Task<GameSessionModel> CreateGameAsync(int size, Guid userId, bool isVsAi)
        {
            var game = new GameSessionModel(size, Guid.NewGuid(), userId, isVsAi);
            await _repository.AddAsync(game);
            return game;
        }

        public async Task<GameSessionModel> GetGameAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MoveStatus> MakeMoveAsync(Guid id, int x, int y, Guid userId)
        {
            GameSessionModel session = await _repository.GetByIdAsync(id);

            if (session == null)
                return MoveStatus.StateError;

            bool vsAi = session.PlayerOId == null;

            MoveStatus moveStatus = session.TryMakeMove(userId, x, y);

            if (moveStatus != MoveStatus.Suсcsess)
                return moveStatus;

            if (session.IsVsAi && session.Result == GameResult.InProgress)
            {
                var (aiX, aiY) = _domainService.GetNextMove(session);
                session.MakeAIMove(aiX, aiY);
            }

            await _repository.UpdateAsync(session);

            return moveStatus;
        }
    }
}
