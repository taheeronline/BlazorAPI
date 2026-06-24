using Microsoft.EntityFrameworkCore;
using MovieManager.API.DTOs.MovieDTOs;
using MovieManager.API.Exceptions;
using MovieManager.API.Models;
using MovieManager.API.Persistence;
using MovieManager.API.Services.Interface;

namespace MovieManager.API.Services.Implementation
{
    public class MovieService : iMovieService
    {
        private readonly MovieDbContext _dbContext;
        private readonly ILogger<MovieService> _logger;

        public MovieService(MovieDbContext dbContext, ILogger<MovieService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MovieDTO>> GetAll()
        {
            try
            {
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .OrderByDescending(m => m.CreatedDate)
                    .ToListAsync();

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all movies.");
                throw new MovieManagerException("Failed to retrieve movies. Please try again later.", 500);
            }
        }

        public async Task<MovieDTO> GetById(int id)
        {
            if (id <= 0) throw new MovieValidationException("Movie ID must be greater than zero.");

            try
            {
                var movie = await _dbContext.Movies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null) throw new MovieNotFoundException(id);

                return MapMovieToDto(movie);
            }
            catch (MovieNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to retrieve movie. Please try again later.", 500);
            }
        }

        public async Task<MovieDTO> CreateMovie(CreateMovieDTO createMovieDto)
        {
            if (createMovieDto is null) throw new MovieValidationException("Movie data cannot be null.");

            try
            {
                var movie = Movie.Create(
                    createMovieDto.Title,
                    createMovieDto.Director,
                    createMovieDto.Genre,
                    createMovieDto.ReleaseDate,
                    createMovieDto.Rating,
                    createMovieDto.CreatedBy // In a real application, this would come from the authenticated user context
                );

                _dbContext.Movies.Add(movie);
                await _dbContext.SaveChangesAsync();

                return MapMovieToDto(movie);
            }
            catch (ArgumentException ex)
            {
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating movie.");
                throw new MovieManagerException("Failed to create movie.", 500);
            }
        }

        public async Task<MovieDTO> UpdateMovie(int id, UpdateMovieDTO updateMovieDto)
        {
            if (id <= 0) throw new MovieValidationException("Movie ID must be greater than zero.");
            if (updateMovieDto is null) throw new MovieValidationException("Updated movie data cannot be null.");

            try
            {
                var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null) throw new MovieNotFoundException(id);

                movie.Update(
                    updateMovieDto.Title,
                    updateMovieDto.Director,
                    updateMovieDto.Genre,
                    updateMovieDto.ReleaseDate,
                    updateMovieDto.Rating,
                    updateMovieDto.ModifiedBy // In a real application, this would come from the authenticated user context
                );

                await _dbContext.SaveChangesAsync();

                return MapMovieToDto(movie);
            }
            catch (MovieNotFoundException) { throw; }
            catch (ArgumentException ex)
            {
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to update movie.", 500);
            }
        }

        public async Task DeleteMovie(int id, int modifiedById)
        {
            if (id <= 0) throw new MovieValidationException("Movie ID must be greater than zero.");
            if (modifiedById <= 0) throw new MovieValidationException("ModifiedBy ID must be greater than zero.");

            try
            {
                var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);

                if (movie is null) throw new MovieNotFoundException(id);

                // Pass the user ID into your domain method
                movie.MarkAsDeleted(modifiedById);

                await _dbContext.SaveChangesAsync();
            }
            catch (MovieNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting movie with ID: {movieId}", id);
                throw new MovieManagerException("Failed to delete movie.", 500);
            }
        }
        
        public async Task<IEnumerable<MovieDTO>> GetByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new MovieValidationException("Search title cannot be empty.");

            try
            {
                var searchPattern = $"%{title}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Title, searchPattern))
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by title.");
                throw new MovieManagerException("Failed to search movies.", 500);
            }
        }

        public async Task<IEnumerable<MovieDTO>> GetByDirector(string director)
        {
            if (string.IsNullOrWhiteSpace(director)) throw new MovieValidationException("Search director name cannot be empty.");

            try
            {
                var searchPattern = $"%{director}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Director, searchPattern))
                    .OrderBy(m => m.Director)
                    .ThenBy(m => m.Title)
                    .ToListAsync();

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by director.");
                throw new MovieManagerException("Failed to search movies.", 500);
            }
        }

        public async Task<IEnumerable<MovieDTO>> GetByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new MovieValidationException("Search genre cannot be empty.");

            try
            {
                var searchPattern = $"%{genre}%";
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Genre, searchPattern))
                    .OrderBy(m => m.Genre)
                    .ThenBy(m => m.Title)
                    .ToListAsync();

                return MapMoviesToDtos(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by genre.");
                throw new MovieManagerException("Failed to search movies.", 500);
            }
        }

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
                CreatedDate = movie.CreatedDate,
                ModifiedDate = movie.ModifiedDate,
                CreatedBy = movie.CreatedByUser?.Id ?? 0,
                ModifiedBy = movie.ModifiedByUser?.Id ?? 0
            };
        }

        private static IEnumerable<MovieDTO> MapMoviesToDtos(IEnumerable<Movie> movies)
        {
            return movies.Select(MapMovieToDto);
        }
    }
}