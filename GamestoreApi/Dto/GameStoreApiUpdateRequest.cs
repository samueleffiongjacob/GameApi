namespace GamestoreApi.Dto
{

    public class GameStoreApiUpdateRequest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Genre { get; set; }
        public required decimal Price { get; set; }
        public required DateOnly ReleaseDate { get; set; }
    }
}