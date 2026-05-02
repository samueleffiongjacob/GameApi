using GamestoreApi.Dto;
using GamestoreApi.Model;

namespace GamestoreApi.Service
{
    public class GameStoreService: IGameStoreCharacterService
    {
        // in merory data store for demonstration purposes
        private List<GameFlowResponse> games = new List<GameFlowResponse>
        {
            new GameFlowResponse { Id = 1, Name = "The Legend of Zelda: Breath of the Wild", Genre = "Action-Adventure", Price = 59.99m, ReleaseDate = new DateOnly(2017, 3, 3) },
            new GameFlowResponse { Id = 2, Name = "Red Dead Redemption 2", Genre = "Action-Adventure", Price = 59.99m, ReleaseDate = new DateOnly(2018, 10, 26) },
            new GameFlowResponse { Id = 3, Name = "The Witcher 3: Wild Hunt", Genre = "RPG", Price = 39.99m, ReleaseDate = new DateOnly(2015, 5, 19) },
            new GameFlowResponse { Id = 4, Name = "Cyberpunk 2077", Genre = "RPG", Price = 59.99m, ReleaseDate = new DateOnly(2020, 12, 10) },
            new GameFlowResponse { Id = 5, Name = "Hades", Genre = "Rogue-like", Price = 24.99m, ReleaseDate = new DateOnly(2020, 9, 17) }
        };

        public async Task<List<GameFlowResponse>> GetAllGamesStoreAsync()
        {
            return await Task.FromResult(games);
        }
        
        public async Task<GameFlowResponse?> GetGameByIdAsync(int id)
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            return await Task.FromResult(game);
        }
        
        public async Task<GameFlowResponse> CreateGameAsync(GameFlowStoreApiCreateRequest store)
        {
            var newGame = new GameFlowResponse 
            { 
                Id = games.Max(g => g.Id) + 1, 
                Name = store.Name, 
                Genre = store.Genre, 
                Price = store.Price, 
                ReleaseDate = store.ReleaseDate 
            };
            games.Add(newGame);
            return await Task.FromResult(newGame);
        }
        
        public async Task<bool> UpdateGameAsync(int id, GameStoreApiUpdateRequest store)
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game == null) return await Task.FromResult(false);
            
            if (!string.IsNullOrEmpty(store.Name)) game.Name = store.Name;
            if (!string.IsNullOrEmpty(store.Genre)) game.Genre = store.Genre;
            if (store.Price > 0) game.Price = store.Price;
            if (store.ReleaseDate != default) game.ReleaseDate = store.ReleaseDate;
            
            return await Task.FromResult(true);
        }
        
        public async Task<bool> DeleteGameAsync(int id)
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game == null) return await Task.FromResult(false);
            
            games.Remove(game);
            return await Task.FromResult(true);
        }
    }
}