# MoviesAPI

**Course:** INET2005 - Web Application Programming I

Read-only Web API for the Reelbox movie catalog. It reads the MoviesAdmin database directly with
[Dapper](https://github.com/DapperLib/Dapper) (a micro-ORM over ADO.NET) and will feed the public,
client-facing site.

## Endpoints

| Method | Route         | Returns                                                                          |
|--------|---------------|----------------------------------------------------------------------------------|
| GET    | `/api/movies` | Every movie with its genre, audience rating, director, studio, poster, trailer, and published review stats |
| GET    | `/api/usersnapshots` | The admin app's UserSnapshot table: each Auth0 user's name, email, roles and login times (Development only) |

## Running locally

1. Start the MoviesAdmin SQL Server container (see the MoviesAdmin repo).
2. Point the API at that database (stored in user secrets, not in `appsettings.json`):

   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=MoviesAdmin;User Id=sa;Password=<sa password>;TrustServerCertificate=True" --project MoviesAPI
   ```

3. Run it and open https://localhost:7017/api/movies:

   ```
   dotnet run --project MoviesAPI --launch-profile https
   ```

## Testing with Bruno

The `bruno/` folder is a [Bruno](https://www.usebruno.com/) collection. In Bruno, choose
**Open Collection**, pick the `bruno` folder, select the **Local** environment, and send
**Movies > Get all movies**; its tests check the status code and the shape of each movie.

From the command line: `cd bruno && npx @usebruno/cli run --env Local --insecure`
(`--insecure` accepts the local development certificate).
