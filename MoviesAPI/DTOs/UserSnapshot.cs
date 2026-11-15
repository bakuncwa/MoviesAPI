namespace MoviesAPI.DTOs
{
    // One row of the admin app's UserSnapshot table: an Auth0 user's name, email and roles, added
    // or updated by the admin app each time that user logs in.
    public class UserSnapshot
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Roles { get; set; }
        public DateTime FirstLoginAt { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}
