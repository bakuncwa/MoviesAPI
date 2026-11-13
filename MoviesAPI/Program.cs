using System.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Database connection: the MoviesAdmin database, read with Dapper. The connection string comes from
// user secrets in Development and from the ConnectionStrings__DefaultConnection environment
// variable elsewhere, never from appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is not configured. Set it with: dotnet user-secrets set " +
        "\"ConnectionStrings:DefaultConnection\" \"<connection string>\" --project MoviesAPI");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
