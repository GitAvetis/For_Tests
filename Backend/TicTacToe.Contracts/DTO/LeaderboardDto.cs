namespace TicTacToe.Contracts.DTO
{
    public class LeaderboardDto
    {
        public Guid UserId { get; set; }
        public double WinRate { get; set; }
        public string Login {  get; set; } = string.Empty;
    }
}
