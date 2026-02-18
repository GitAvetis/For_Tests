using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Contracts.DTO;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Mappers
{
    public class UserMapper
    {
        public static UserDto ToDto(UserModel model)
        {
            return new UserDto
            {
                Id = model.Id,
                Login = model.Login,
                PasswordHash = model.PasswordHash
            };
        }
    }
}
