using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Application;
using TicTacToe.Application.Interfaces;

namespace TicTacToe.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            bool result = await _authService.RegisterAsync(request);

            if (!result)
                return BadRequest("User already exists");

            return Ok("User registered successfully");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login()
        {
            // В Basic Auth логин происходит автоматически через header.
            // Если мы сюда дошли — значит авторизация прошла.

            return Ok("Authorized");
        }
    }
}
