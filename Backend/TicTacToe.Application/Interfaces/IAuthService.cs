namespace TicTacToe.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(SignUpRequest request);
        Task<Guid?> AuthenticateAsync(string login, string password);
    }
}
