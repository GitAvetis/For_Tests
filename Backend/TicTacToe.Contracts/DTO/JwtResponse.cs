namespace TicTacToe.Contracts.DTO
{
    public class JwtResponse
    {
        public string TokenType { get; set; } = "Bearer";
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
