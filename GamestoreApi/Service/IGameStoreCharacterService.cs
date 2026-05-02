using GamestoreApi.Dto;

namespace GamestoreApi.Service;

public interface IGameStoreCharacterService
{
    Task<List<GameFlowResponse>> GetAllGamesStoreAsync();
    Task<GameFlowResponse?> GetGameByIdAsync(int id);
    Task<GameFlowResponse> CreateGameAsync(GameFlowStoreApiCreateRequest store);
    Task<bool> UpdateGameAsync(int id, GameStoreApiUpdateRequest store);
    Task<bool> DeleteGameAsync(int id);   
}
