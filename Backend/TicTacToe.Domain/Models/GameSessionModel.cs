namespace TicTacToe.Domain.Models
{
    public enum GameResult
    {
        InProgress,
        Draw,
        WinX,
        WinO
    }

    public class GameSessionModel
    {
        public Guid Id { get; private set; }
        public GameFieldModel Field { get; private set; }
        public CellState CurrentPlayer { get; private set; }
        public GameResult Result { get; private set; }
        public GameStatus Status { get; private set; }
        public Guid? PlayerXId { get; private set; }
        public Guid? PlayerOId { get; private set; }
        public bool IsVsAi { get; private set; }

        public GameSessionModel(int dimension, Guid id, Guid creatorId, bool isVsAi)
        {
            Id = id;
            Field = new GameFieldModel(dimension);
            CurrentPlayer = CellState.X;
            Result = GameResult.InProgress;
            PlayerXId = creatorId;
            IsVsAi = isVsAi;
            Status = isVsAi ? GameStatus.InProgress : GameStatus.WaitingForOpponent;
        }

        private GameSessionModel(
            Guid id,
            GameFieldModel field,
            CellState currentPlayer,
            GameResult result,
            GameStatus status,
            Guid? playerXId,
            Guid? playerOId,
            bool isVsAi)
        {
            Id = id;
            Field = field;
            CurrentPlayer = currentPlayer;
            Result = result;
            Status = status;
            PlayerXId = playerXId;
            PlayerOId = playerOId;
            IsVsAi = isVsAi;
        }

        public static GameSessionModel Restore(
            Guid id,
            CellState[,] field,
            CellState currentPlayer,
            GameResult result,
            GameStatus status,
            Guid? playerXId,
            Guid? playerOId,
            bool isVsAi)
        {
            var gameField = new GameFieldModel(field);
            return new GameSessionModel(
                id,
                gameField,
                currentPlayer,
                result,
                status,
                playerXId,
                playerOId,
                isVsAi);
        }

        public MoveStatus TryMakeMove(Guid userId, int x, int y)
        {
            if (Result != GameResult.InProgress)
                return MoveStatus.GameIsOver;

            if (!IsVsAi && PlayerOId == null)
                return MoveStatus.GameNotStarted;

            if (userId != PlayerXId && userId != PlayerOId)
                return MoveStatus.NotYourGame;

            var playerSymbol = userId == PlayerXId ? CellState.X : CellState.O;

            if (playerSymbol != CurrentPlayer)
                return MoveStatus.NotYourTurn;

            MoveStatus moveStatus = Field.MakeMove(x, y, CurrentPlayer);
            if (moveStatus == MoveStatus.Suсcsess)
                UpdateGameStateAfterMove();

            return moveStatus;
        }

        public MoveStatus MakeAIMove(int x, int y)
        {
            if (!IsVsAi)
                return MoveStatus.StateError;

            if (Result != GameResult.InProgress)
                return MoveStatus.GameIsOver;

            if (CurrentPlayer != CellState.O)
                return MoveStatus.NotYourTurn;

            var moveStatus = Field.MakeMove(x, y, CellState.O);

            if (moveStatus == MoveStatus.Suсcsess)
                UpdateGameStateAfterMove();

            return moveStatus;
        }

        private void UpdateGameStateAfterMove()
        {
            if (Field.CheckWin() != CellState.Empty)
            {
                Result = CurrentPlayer == CellState.X
                    ? GameResult.WinX
                    : GameResult.WinO;
                Status = GameStatus.Finished;
            }
            else if (Field.IsFieldFull())
            {
                Result = GameResult.Draw;
                Status = GameStatus.Finished;
            }
            else
            {
                SwitchPlayer();
            }
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == CellState.X ? CellState.O : CellState.X;
        }

        public bool Join(Guid userId)
        {
            if (PlayerOId != null)
                return false;

            if (userId == PlayerXId)
                return false;

            PlayerOId = userId;
            Status = GameStatus.InProgress;
            return true;
        }
    }
}
