namespace TicTacToe.Domain.Models
{
    public class UserModel
    {
        public Guid Id { get; }
        public string Login { get; }
        public string PasswordHash { get; }

        private UserModel(Guid id, string login, string passwordHash)
        {
            Id = id;
            Login = login;
            PasswordHash = passwordHash;
        }

        public static UserModel Restore(Guid id, string login, string passwordHash)
        {
            return new UserModel(id, login, passwordHash);
        }

        public static UserModel Create(Guid id, string login, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return new UserModel(id, login, passwordHash);
        }

        public bool ValidatePassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
