using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GamestoreApi.Dto;
using GamestoreApi.Service;
using Microsoft.AspNetCore.Authorization;

namespace GamestoreApi.GameStoreApiController
{
    [Authorize]
    [Route("v1/api/[controller]")]
    [ApiController]
    public class GameStoreApiController(IGameStoreCharacterService gameStoreService) : ControllerBase
    {
       
        [HttpGet]
        public async Task<ActionResult<List<GameFlowResponse>>> GetAllGames()
        {
            var games = await gameStoreService.GetAllGamesStoreAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameFlowResponse>> GetGameById(int id)
        {
            var game = await gameStoreService.GetGameByIdAsync(id);
            return game is not null ? Ok(game) : NotFound("Game with the specified ID was not found.");
        }

        [HttpPost]
        public async Task<ActionResult<GameFlowResponse>> CreateGame(GameFlowStoreApiCreateRequest request)
        {   
            var newGame = await gameStoreService.CreateGameAsync(request);
            return CreatedAtAction(nameof(GetGameById), new { id = newGame.Id }, newGame);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGame(int id, GameStoreApiUpdateRequest request)
        {
            var success = await gameStoreService.UpdateGameAsync(id, request);
            return success ? NoContent() : NotFound("Game with the specified ID was not found.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGame(int id)
        {
            var success = await gameStoreService.DeleteGameAsync(id);
            return success ? NoContent() : NotFound("Game with the specified ID was not found.");
        }   
    }
}
