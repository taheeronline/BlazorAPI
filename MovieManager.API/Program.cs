using Microsoft.EntityFrameworkCore;
using MovieManager.API.Middleware;
using MovieManager.API.Persistence;
using MovieManager.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI documentation
builder.Services.AddOpenApi();

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Add application services with dependency injection
builder.Services.AddScoped<iUserService, UserService>();
builder.Services.AddScoped<iMovieService, MovieService>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
