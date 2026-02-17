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

        public GameSessionModel(int dimension, Guid id)
        {
            Id = id;
            Field = new GameFieldModel(dimension);
            CurrentPlayer = CellState.X;
            Result = GameResult.InProgress;
        }

        private GameSessionModel(Guid id, GameFieldModel field, CellState currentPlayer, GameResult result)
        {
            Id = id;
            Field = field;
            CurrentPlayer = currentPlayer;
            Result = result;
        }

        public static GameSessionModel Restore(Guid id, CellState[,] field, CellState currentPlayer, GameResult result)
        {
            var gameField = new GameFieldModel(field);
            return new GameSessionModel(id, gameField, currentPlayer, result);
        }

        public MoveStatus TryMakeMove(int x, int y)
        {
            if (Result != GameResult.InProgress)
                return MoveStatus.GameIsOver;

            MoveStatus moveStatus = Field.MakeMove(x, y, CurrentPlayer);

            if (moveStatus == MoveStatus.Suсcsess)
            {
                if (Field.CheckWin() != CellState.Empty)
                {
                    Result = CurrentPlayer == CellState.X ? GameResult.WinX : GameResult.WinO;
                }
                else if (Field.IsFieldFull())
                {
                    Result = GameResult.Draw;
                }
                else
                {
                    SwitchPlayer();
                }
            }
            return moveStatus;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == CellState.X ? CellState.O : CellState.X;
        }
    }
}
