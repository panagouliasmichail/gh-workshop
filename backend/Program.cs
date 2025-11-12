using backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Sample data
var directors = new List<Director>
{
    new(1, "Christopher Nolan", "British-American", 1970),
    new(2, "Francis Ford Coppola", "American", 1939),
    new(3, "Quentin Tarantino", "American", 1963),
    new(4, "Frank Darabont", "American", 1959)
};

var movies = new List<Movie>
{
    new(1, "Inception", 1, 2010, "Sci-Fi"),
    new(2, "The Godfather", 2, 1972, "Crime"),
    new(3, "Pulp Fiction", 3, 1994, "Crime"),
    new(4, "The Shawshank Redemption", 4, 1994, "Drama"),
    new(5, "The Dark Knight", 1, 2008, "Action")
};

// Get all directors
app.MapGet("/directors", () => directors)
    .WithName("GetDirectors")
    .WithOpenApi();

// Get director by id
app.MapGet("/directors/{id:int}", (int id) =>
{
    var director = directors.FirstOrDefault(d => d.Id == id);
    return director is not null ? Results.Ok(director) : Results.NotFound();
})
    .WithName("GetDirectorById")
    .WithOpenApi();

// Get directors of movies (all unique directors who have directed movies)
app.MapGet("/movies/directors", () =>
{
    var movieDirectorIds = movies.Select(m => m.DirectorId).Distinct();
    var movieDirectors = directors.Where(d => movieDirectorIds.Contains(d.Id)).ToList();
    return movieDirectors;
})
    .WithName("GetDirectorsOfMovies")
    .WithOpenApi();

// Get all movies
app.MapGet("/movies", () => movies)
    .WithName("GetMovies")
    .WithOpenApi();

// Get movies by director id
app.MapGet("/directors/{id:int}/movies", (int id) =>
{
    var directorMovies = movies.Where(m => m.DirectorId == id).ToList();
    return directorMovies.Any() ? Results.Ok(directorMovies) : Results.NotFound();
})
    .WithName("GetMoviesByDirector")
    .WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
