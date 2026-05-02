namespace GamestoreApi.Model
{

    public class GameStoreApiModel
    {
        public int Id { get;set; }
        public required string Name { get; set; } 
        public required string Genre { get; set; }
        public required decimal Price { get; set; } 
        public required DateOnly ReleaseDate { get; set; }
    }
}