namespace MoviesAPI.DTOs
{
    // "PG-13" style rating with its description ("Parents Strongly Cautioned").
    public class AudienceRating
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
