namespace TicTacToe.Contracts.DTO
{
    public class CreateGameRequestDto
    {
        public int Size { get; set; }
        public bool IsVsAi { get; set; }
    }
}
