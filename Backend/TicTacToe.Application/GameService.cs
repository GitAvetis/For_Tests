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

        public async Task<GameSessionModel> CreateGameAsync(int size)
        {
            var game = new GameSessionModel(size,Guid.NewGuid());
            await _repository.AddAsync(game);
            return game;
        }

        public async Task<GameSessionModel> GetGameAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MoveStatus> MakeMoveAsync(Guid id, int x, int y, bool vsAi)
        {
            GameSessionModel session = await _repository.GetByIdAsync(id);

            if (session == null)
                return MoveStatus.StateError;

            MoveStatus moveStatus = session.TryMakeMove(x, y);

            if (moveStatus == MoveStatus.Suсcsess && vsAi && session.Result == GameResult.InProgress)
            {
                (int aiX, int aiY) = _domainService.GetNextMove(session);
                session.TryMakeMove(aiX, aiY);
            }

            //if (!_domainService.ValidateField(session))
            //    return MoveStatus.StateError;

            //if (_domainService.IsGameOver(session))
            //    return MoveStatus.GameIsOver;

            _repository.UpdateAsync(session);
            return moveStatus;
        }
    }
}
