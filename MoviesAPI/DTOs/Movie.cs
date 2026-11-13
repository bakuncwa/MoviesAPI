using System.Text.Json.Serialization;

namespace MoviesAPI.DTOs
{
    // One movie as GET /api/movies returns it, read from the MoviesAdmin database (Movies table plus
    // its lookups, trailer and published review stats).
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Synopsis { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int? RuntimeMinutes { get; set; }

        // Poster to display: the uploaded file, else the external PosterUrl, else null.
        // Filled in by MoviesController from the two raw columns below.
        public string? PosterImageUrl { get; set; }

        // Raw poster columns, used to work out PosterImageUrl; not part of the JSON.
        [JsonIgnore]
        public string? PosterImagePath { get; set; }
        [JsonIgnore]
        public string? PosterUrl { get; set; }

        public string? TrailerUrl { get; set; }

        // Average of the published critic reviews (0 to 5, one decimal), null when there are none.
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public Genre Genre { get; set; } = null!;
        public AudienceRating AudienceRating { get; set; } = null!;
        // Optional on a movie, so null when not set.
        public Director? Director { get; set; }
        public Studio? Studio { get; set; }
    }
}
