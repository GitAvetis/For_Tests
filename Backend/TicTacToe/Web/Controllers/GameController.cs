using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Models;
using TicTacToe.Contracts.DTO;
using TicTacToe.Application;

namespace TicTacToe.Web.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        //[HttpPost]
        //public ActionResult<GameDto> CreateGame([FromQuery] int size)
        //{
        //    GameSessionModel session = _gameService.CreateGame(size);
        //    return Ok(GameMapper.ToDto(session));
        //}

        //[HttpGet("{id}")]
        //public ActionResult<GameDto> GetGame(Guid id)
        //{
        //    GameSessionModel? session = _gameService.GetGame(id);
        //    if (session == null)
        //        return NotFound();
        //    return Ok(GameMapper.ToDto(session));
        //}

        //[HttpPost("{id}/move")]
        //public ActionResult<GameDto> MakeMove(Guid id, [FromBody] MoveRequestDto moveRequest)
        //{
        //    var moveResult = _gameService.MakeMove(id, moveRequest.X, moveRequest.Y, moveRequest.VsAi);

        //    if (moveResult == MoveStatus.StateError)
        //        return NotFound($"Game {id} not found.");

        //    GameSessionModel? session = _gameService.GetGame(id);
        //    GameDto gameDto = GameMapper.ToDto(session);

        //    if (moveResult == MoveStatus.CellIsNotEmpty)
        //    {
        //        ErrorResponseDto errorResponse = new ErrorResponseDto
        //        {
        //            Message = "ERROR: Cell already occupied.",
        //            Game = gameDto
        //        };
        //        return BadRequest(errorResponse);
        //    }

        //    if (moveResult == MoveStatus.IncorrectCoordinates)
        //    {
        //        ErrorResponseDto errorResponse = new ErrorResponseDto
        //        {
        //            Message = "ERROR: Move out of bounds.",
        //            Game = gameDto
        //        };
        //        return BadRequest(errorResponse);
        //    }

        //    if(moveResult == MoveStatus.GameIsOver)
        //    {
        //        ErrorResponseDto errorResponse = new ErrorResponseDto
        //        {
        //            Message = $"Game over. Result of game: {session.Result}",
        //            Game = gameDto
        //        };
        //        return BadRequest(errorResponse);
        //    }

        //    return Ok(gameDto);
        //}
    }
}
