using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicTacToe.Application;
using TicTacToe.Application.Interfaces;
using TicTacToe.Contracts.DTO;
using TicTacToe.Domain.Models;

namespace TicTacToe.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId!);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto requestDto)
        {
            Guid userId = GetCurrentUserId();

            GameSessionModel session = await _gameService.CreateGameAsync(
                requestDto.Size,
                userId,
                requestDto.IsVsAi);
            return Ok(GameMapper.ToDto(session));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            GameSessionModel? session = await _gameService.GetGameAsync(id);
            if (session == null)
                return NotFound();
            return Ok(GameMapper.ToDto(session));
        }

        [HttpPost("{id}/move")]
        public async Task<IActionResult> MakeMove(Guid gameId, [FromBody] MoveRequestDto moveRequest)
        {
            Guid userId = GetCurrentUserId();

            var moveResult = await _gameService.MakeMoveAsync(gameId, moveRequest.X, moveRequest.Y, userId);

            if (moveResult == MoveStatus.StateError)
                return NotFound($"Game {gameId} not found.");

            GameSessionModel? session = await _gameService.GetGameAsync(gameId);
            if (session == null)
                return NotFound();

            GameDto gameDto = GameMapper.ToDto(session);

            if (moveResult != MoveStatus.Suсcsess)
                return BadRequest(new ErrorResponseDto
                {
                    Message = moveResult.ToString(),
                    Game = gameDto
                });

            return Ok(gameDto);
        }
    }
}
