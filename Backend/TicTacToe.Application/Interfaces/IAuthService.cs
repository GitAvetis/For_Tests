namespace TicTacToe.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(SingUpRequest request);
        Task<Guid?> AuthenticateAsync(string login, string password);
    }
}
