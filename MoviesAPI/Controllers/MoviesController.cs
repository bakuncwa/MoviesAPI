using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;

namespace MoviesAPI.Controllers
{
    // Read-only movie data for the public site, queried straight from the MoviesAdmin database with
    // Dapper (plain SQL mapped onto the DTOs) instead of Entity Framework.
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly string? _adminBaseUrl;

        public MoviesController(IDbConnection db, IConfiguration configuration)
        {
            _db = db;
            // Posters uploaded to the admin app's wwwroot are stored as relative paths
            // ("images/movies/x.jpg"), so they need the admin app's address in front.
            _adminBaseUrl = configuration["AdminApp:BaseUrl"]?.TrimEnd('/');
        }

        // GET /api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAllMovies()
        {
            // Each SELECT block after the first starts with an Id column; Dapper splits the row there
            // (splitOn) and maps each block onto its own DTO. Director and Studio are LEFT JOINs, so
            // their blocks come back null when the movie has none.
            var sql = @"SELECT m.Id, m.Title, m.Synopsis, m.ReleaseDate, m.RuntimeMinutes,
                       m.PosterImagePath, m.PosterUrl,
                       t.YouTubeUrl AS TrailerUrl,
                       ROUND(rv.AverageRating, 1) AS AverageRating,
                       ISNULL(rv.ReviewCount, 0) AS ReviewCount,
                       g.Id, g.Name,
                       ar.Id, ar.Name, ar.Description,
                       d.Id, d.Name, d.Bio,
                       s.Id, s.Name
                FROM Movies m
                INNER JOIN Genres g ON m.GenreId = g.Id
                INNER JOIN AudienceRatings ar ON m.AudienceRatingId = ar.Id
                LEFT JOIN Directors d ON m.DirectorId = d.Id
                LEFT JOIN Studios s ON m.StudioId = s.Id
                LEFT JOIN Trailers t ON t.MovieId = m.Id
                LEFT JOIN (
                    SELECT MovieId, AVG(CAST(Rating AS float)) AS AverageRating, COUNT(*) AS ReviewCount
                    FROM Reviews
                    WHERE IsPublished = 1
                    GROUP BY MovieId
                ) rv ON rv.MovieId = m.Id
                ORDER BY m.ReleaseDate DESC, m.Title";

            // <Table 1, Table 2, ..., Output>
            var movies = await _db.QueryAsync<Movie, Genre, AudienceRating, Director, Studio, Movie>(
                sql,
                (movie, genre, audienceRating, director, studio) =>
                {
                    movie.Genre = genre;
                    movie.AudienceRating = audienceRating;
                    movie.Director = director;
                    movie.Studio = studio;
                    movie.PosterImageUrl = ResolvePoster(movie);
                    return movie;
                },
                splitOn: "Id,Id,Id,Id");

            return Ok(movies);
        }

        // Same order as the admin app's MoviePosterResolver: uploaded file first, then the external
        // PosterUrl. Blob uploads are already absolute URLs; wwwroot uploads are relative.
        private string? ResolvePoster(Movie movie)
        {
            if (!string.IsNullOrWhiteSpace(movie.PosterImagePath))
            {
                if (Uri.IsWellFormedUriString(movie.PosterImagePath, UriKind.Absolute) || _adminBaseUrl is null)
                {
                    return movie.PosterImagePath;
                }
                return $"{_adminBaseUrl}/{movie.PosterImagePath.TrimStart('/')}";
            }

            return string.IsNullOrWhiteSpace(movie.PosterUrl) ? null : movie.PosterUrl;
        }
    }
}
