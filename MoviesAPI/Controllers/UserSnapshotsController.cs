using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;

namespace MoviesAPI.Controllers
{
    // The admin app's UserSnapshot table, read with Dapper. Development only: it lists every user's
    // email, so outside Development it returns 404 until the API has authentication.
    [Route("api/[controller]")]
    [ApiController]
    public class UserSnapshotsController : ControllerBase
    {
        private readonly IDbConnection _db;
        private readonly IWebHostEnvironment _environment;

        public UserSnapshotsController(IDbConnection db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        // GET /api/usersnapshots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSnapshot>>> GetAllUserSnapshots()
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            var sql = @"SELECT Id, UserId, Name, Email, Roles, FirstLoginAt, LastLoginAt
                FROM UserSnapshot
                ORDER BY Id";
            var snapshots = await _db.QueryAsync<UserSnapshot>(sql);

            return Ok(snapshots);
        }
    }
}
