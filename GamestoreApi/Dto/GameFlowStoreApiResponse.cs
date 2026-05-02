namespace GamestoreApi.Dto
{
    /* A Dto is a content between the client and server since it represents
       a shared agreement about how the data will be transferred or used.
    */
    public class GameFlowResponse
    {
        public int Id { get;set; }
        public required string Name { get; set; } 
        public required string Genre { get; set; }
        public required decimal Price { get; set; } 
        public required DateOnly ReleaseDate { get; set; }
    }
}

/* A Dto is a content between the client and server since it represents
a shared agreement about how the data will be transferred or used.
*/

// public class GameFlow(
//     int Id,
//     string Name,
//     string Genre,
//     decimal Price,
//     DateOnly ReleaseDate
// );


