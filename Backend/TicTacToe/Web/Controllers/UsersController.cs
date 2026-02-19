using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Application.Interfaces;
using TicTacToe.Application.Mappers;

namespace TicTacToe.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            return Ok(UserMapper.ToDto(user));
        }
    }
}
