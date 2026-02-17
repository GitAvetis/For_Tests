namespace TicTacToe.Contracts.DTO
{
    public class MoveRequestDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool VsAi { get; set; }
    }
}
