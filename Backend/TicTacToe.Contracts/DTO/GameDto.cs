namespace TicTacToe.Contracts.DTO
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public int Size { get; set; }
        public string CurrentPlayer { get; set; }
        public string Result { get; set; }
        public string[] PrettyField { get; set; } = default!;
        public Guid? PlayerXId { get; set; }
        public Guid? PlayerOId { get; set; }
        public Guid? WinnerId { get; set; }
        public bool IsVsAi { get; set; }
    }
}
