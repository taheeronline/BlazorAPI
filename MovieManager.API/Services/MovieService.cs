using Microsoft.EntityFrameworkCore;
using MovieManager.API.DTOs;
using MovieManager.API.Exceptions;
using MovieManager.API.Models;
using MovieManager.API.Persistence;

namespace MovieManager.API.Services
{
    /// <summary>
    /// Service implementation for Movie-related operations.
    /// Handles CRUD operations and search functionality with database interactions.
    /// </summary>
    public class MovieService : iMovieService
    {
        private readonly MovieDbContext _dbContext;
        private readonly ILogger<MovieService> _logger;

        /// <summary>
        /// Initializes a new instance of the MovieService class.
        /// </summary>
        /// <param name="dbContext">The database context for accessing movie data.</param>
        /// <param name="logger">Logger for recording service operations.</param>
        public MovieService(MovieDbContext dbContext, ILogger<MovieService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all movies from the database.
        /// </summary>
        /// <returns>Collection of all movies.</returns>
        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync()
        {
            _logger.LogInformation("Retrieving all movies from database.");

            try
            {
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .OrderByDescending(m => m.Created)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {count} movies from database.", movies.Count);

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all movies.");
                throw new MovieManagerException("Failed to retrieve movies. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Retrieves a movie by its unique identifier.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>Movie data if found. Throws MovieNotFoundException (404) if not found.</returns>
        public async Task<MovieDTO> GetMovieByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new MovieValidationException("Movie ID cannot be empty.");
            }

            _logger.LogInformation("Retrieving movie with ID: {movieId}", id);

            try
            {
                var movie = await _dbContext.Movies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null)
                {
                    _logger.LogWarning("Movie with ID {movieId} not found.", id);
                    throw new MovieNotFoundException(id);
                }

                _logger.LogInformation("Successfully retrieved movie with ID: {movieId}", id);

                return MapMovieToDto(movie);
            }
            catch (MovieNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to retrieve movie. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Creates a new movie in the database.
        /// </summary>
        /// <param name="createMovieDto">The movie data to create.</param>
        /// <returns>The created movie with assigned Id.</returns>
        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO createMovieDto)
        {
            if (createMovieDto is null)
            {
                throw new MovieValidationException("Movie data cannot be null.");
            }

            _logger.LogInformation("Creating new movie: Title='{title}', Director='{director}'", 
                createMovieDto.Title, createMovieDto.Director);

            try
            {
                // Use the Movie.Create method for domain validation
                var movie = Movie.Create(
                    createMovieDto.Title,
                    createMovieDto.Director,
                    createMovieDto.Genre,
                    createMovieDto.ReleaseDate,
                    createMovieDto.Rating
                );

                _dbContext.Movies.Add(movie);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully created movie with ID: {movieId}", movie.Id);

                return MapMovieToDto(movie);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating movie.");
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating movie.");
                throw new MovieManagerException("Failed to create movie. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Updates an existing movie in the database.
        /// </summary>
        /// <param name="id">The movie identifier to update.</param>
        /// <param name="updateMovieDto">The updated movie data.</param>
        /// <returns>The updated movie data.</returns>
        public async Task<MovieDTO> UpdateMovieAsync(Guid id, UpdateMovieDTO updateMovieDto)
        {
            if (id == Guid.Empty)
            {
                throw new MovieValidationException("Movie ID cannot be empty.");
            }

            if (updateMovieDto is null)
            {
                throw new MovieValidationException("Updated movie data cannot be null.");
            }

            _logger.LogInformation("Updating movie with ID: {movieId}", id);

            try
            {
                var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null)
                {
                    _logger.LogWarning("Movie with ID {movieId} not found for update.", id);
                    throw new MovieNotFoundException(id);
                }

                // Use the Movie.Update method for domain validation
                movie.Update(
                    updateMovieDto.Title,
                    updateMovieDto.Director,
                    updateMovieDto.Genre,
                    updateMovieDto.ReleaseDate,
                    updateMovieDto.Rating
                );

                _dbContext.Movies.Update(movie);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated movie with ID: {movieId}", id);

                return MapMovieToDto(movie);
            }
            catch (MovieNotFoundException)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating movie with ID: {movieId}", id);
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to update movie. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Deletes a movie from the database by its identifier.
        /// </summary>
        /// <param name="id">The movie identifier to delete.</param>
        public async Task DeleteMovieAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new MovieValidationException("Movie ID cannot be empty.");
            }

            _logger.LogInformation("Deleting movie with ID: {movieId}", id);

            try
            {
                var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null)
                {
                    _logger.LogWarning("Movie with ID {movieId} not found for deletion.", id);
                    throw new MovieNotFoundException(id);
                }

                _dbContext.Movies.Remove(movie);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted movie with ID: {movieId}", id);
            }
            catch (MovieNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to delete movie. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Searches for movies by title (case-insensitive partial match).
        /// </summary>
        /// <param name="title">The movie title to search for.</param>
        /// <returns>Collection of movies matching the title search.</returns>
        public async Task<IEnumerable<MovieDTO>> SearchMoviesByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new MovieValidationException("Search title cannot be empty.");
            }

            _logger.LogInformation("Searching for movies with title containing: '{title}'", title);

            try
            {
                var searchPattern = $"%{title}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Title, searchPattern))
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                _logger.LogInformation("Search found {count} movies matching title '{title}'.", movies.Count, title);

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching movies by title: '{title}'", title);
                throw new MovieManagerException("Failed to search movies by title. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Searches for movies by director name (case-insensitive partial match).
        /// </summary>
        /// <param name="director">The director name to search for.</param>
        /// <returns>Collection of movies matching the director search.</returns>
        public async Task<IEnumerable<MovieDTO>> SearchMoviesByDirectorAsync(string director)
        {
            if (string.IsNullOrWhiteSpace(director))
            {
                throw new MovieValidationException("Search director name cannot be empty.");
            }

            _logger.LogInformation("Searching for movies by director containing: '{director}'", director);

            try
            {
                var searchPattern = $"%{director}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Director, searchPattern))
                    .OrderBy(m => m.Director)
                    .ThenBy(m => m.Title)
                    .ToListAsync();

                _logger.LogInformation("Search found {count} movies by director '{director}'.", movies.Count, director);

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching movies by director: '{director}'", director);
                throw new MovieManagerException("Failed to search movies by director. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Searches for movies by genre (case-insensitive partial match).
        /// </summary>
        /// <param name="genre">The genre to search for.</param>
        /// <returns>Collection of movies matching the genre search.</returns>
        public async Task<IEnumerable<MovieDTO>> SearchMoviesByGenreAsync(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
            {
                throw new MovieValidationException("Search genre cannot be empty.");
            }

            _logger.LogInformation("Searching for movies with genre containing: '{genre}'", genre);

            try
            {
                var searchPattern = $"%{genre}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Genre, searchPattern))
                    .OrderBy(m => m.Genre)
                    .ThenBy(m => m.Title)
                    .ToListAsync();

                _logger.LogInformation("Search found {count} movies with genre '{genre}'.", movies.Count, genre);

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching movies by genre: '{genre}'", genre);
                throw new MovieManagerException("Failed to search movies by genre. Please try again later.", 500);
            }
        }

        /// <summary>
        /// Maps a single Movie entity to a MovieDTO.
        /// </summary>
        /// <param name="movie">The movie entity to map.</param>
        /// <returns>Mapped MovieDTO object.</returns>
        private static MovieDTO MapMovieToDto(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Director = movie.Director,
                Genre = movie.Genre,
                ReleaseDate = movie.ReleaseDate,
                Rating = movie.Rating,
                Created = movie.Created,
                LastModified = movie.LastModified
            };
        }

        /// <summary>
        /// Maps a collection of Movie entities to a collection of MovieDTOs.
        /// </summary>
        /// <param name="movies">The collection of movie entities to map.</param>
        /// <returns>Collection of mapped MovieDTO objects.</returns>
        private static IEnumerable<MovieDTO> MapMoviesToDtos(IEnumerable<Movie> movies)
        {
            return movies.Select(MapMovieToDto);
        }
    }
}
