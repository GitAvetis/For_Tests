using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Ai
{
    public class MiniMax
    {
        public (int bestX, int bestY) GetBestMove(CellState player, CellState[,] board)
        {
            int bestScore = int.MinValue;
            int bestX = -1;
            int bestY = -1;
            int _size = board.GetLength(0);

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (board[i, j] == CellState.Empty)
                    {
                        board[i, j] = player;

                        int score = MinimaxAlg(board, 0, false, player);

                        board[i, j] = CellState.Empty;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestX = i;
                            bestY = j;
                        }
                    }
                }
            }

            return (bestX, bestY);
        }

        public CellState CheckWin(CellState[,] board)
        {
            CellState playerState;
            bool isXWin = HasWinningLine(CellState.X, board);

            if (isXWin)
                playerState = CellState.X;
            else
                playerState = HasWinningLine(CellState.O, board) ? CellState.O : CellState.Empty;

            return playerState;
        }

        private int MinimaxAlg(CellState[,] board, int depth, bool isMaximizing, CellState aiPlayer)
        {
            CellState winner = CheckWin(board);

            if (winner == aiPlayer)
                return 10 - depth;

            if (winner != CellState.Empty && winner != aiPlayer)
                return depth - 10;

            if (IsFieldFull(board))
                return 0;

            int size = board.GetLength(0);
            CellState opponent = aiPlayer == CellState.X ? CellState.O : CellState.X;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (board[i, j] == CellState.Empty)
                        {
                            board[i, j] = aiPlayer;
                            int score = MinimaxAlg(board, depth + 1, false, aiPlayer);
                            board[i, j] = CellState.Empty;
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (board[i, j] == CellState.Empty)
                        {
                            board[i, j] = opponent;
                            int score = MinimaxAlg(board, depth + 1, true, aiPlayer);
                            board[i, j] = CellState.Empty;
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }

                return bestScore;
            }
        }

        public bool IsFieldFull(CellState[,] board)
        {
            int _size = board.GetLength(0);
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (board[i, j] == CellState.Empty)
                        return false;
                }
            }
            return true;
        }

        private static bool HasWinningLine(CellState playerState, CellState[,] board)
        {
            int _size = board.GetLength(0);

            bool winByDiag1 = true;
            bool winByDiag2 = true;

            for (int i = 0; i < _size; i++)
            {
                bool winByRow = true;
                bool winByCol = true;

                for (int j = 0; j < _size; j++)
                {
                    if (board[i, j] != playerState)
                    {
                        winByRow = false;
                    }

                    if (board[j, i] != playerState)
                    {
                        winByCol = false;
                    }
                }

                if (winByCol || winByRow)
                    return true;

                if (board[i, i] != playerState)
                {
                    winByDiag1 = false;
                }

                if (board[i, _size - 1 - i] != playerState)
                {
                    winByDiag2 = false;
                }
            }

            if (winByDiag1 || winByDiag2)
                return true;
            else
                return false;
        }
    }
}
