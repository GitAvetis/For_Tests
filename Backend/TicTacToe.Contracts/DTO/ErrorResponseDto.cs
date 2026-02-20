namespace TicTacToe.Contracts.DTO
{
    public class ErrorResponseDto
    {
        public required string Message { get; set; }
        public required GameDto Game { get; set; }
    }
}