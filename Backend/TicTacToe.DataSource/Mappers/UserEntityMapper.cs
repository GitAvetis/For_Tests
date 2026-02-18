using TicTacToe.DataSource.Entitys;
using TicTacToe.Domain.Models;

namespace TicTacToe.DataSource.Mappers
{
    internal class UserEntityMapper
    {
        public static UserEntity ToUserEntity(UserModel user)
        {
            return new UserEntity
            {
                Id = user.Id,
                Login = user.Login,
                Password = user.PasswordHash
            };
        }

        public static UserModel ToUserModel(UserEntity entity)
        {
            return UserModel.Create(entity.Id, entity.Login, entity.Password);
        }
    }
}
