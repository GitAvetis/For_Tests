namespace TicTacToe.Contracts.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get;set; } = string.Empty;
    }
}
