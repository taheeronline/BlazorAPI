using Microsoft.EntityFrameworkCore;
using BlazorAPI.API.DTOs.MovieDTOs;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Implementation
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

        public async Task<PagedResult<MovieDTO>> GetAll(int page, int pageSize)
        {
            try
            {
                // 1. Count the total records BEFORE applying pagination
                var totalCount = await _dbContext.Movies.CountAsync();

                // 2. Fetch only the requested page of data, maintaining your sorting
                var movies = await _dbContext.Movies
                    .AsNoTracking()
                    .OrderByDescending(m => m.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 3. Map the retrieved database models to DTOs
                var movieDtos = MapMoviesToDtos(movies);

                // 4. Return the complete PagedResult package
                return new PagedResult<MovieDTO>
                {
                    Items = movieDtos,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                // Added the page number to the log for better debugging
                _logger.LogError(ex, "Error occurred while retrieving movies for page {Page}.", page);
                throw new BlazorAPIException("Failed to retrieve movies. Please try again later.", 500);
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
                throw new BlazorAPIException("Failed to retrieve movie. Please try again later.", 500);
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
                throw new BlazorAPIException("Failed to create movie.", 500);
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
                throw new BlazorAPIException("Failed to update movie.", 500);
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
                throw new BlazorAPIException("Failed to delete movie.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByTitle(string title, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new MovieValidationException("Search title cannot be empty.");

            try
            {
                var searchPattern = $"%{title}%";

                // 1. Build the base filtered query
                var query = _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Title, searchPattern));

                // 2. Count the total matching records before paginating
                var totalCount = await query.CountAsync();

                // 3. Apply sorting and pagination, then execute
                var movies = await query
                    .OrderBy(m => m.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 4. Return the complete package
                return new PagedResult<MovieDTO>
                {
                    Items = MapMoviesToDtos(movies),
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by title for page {Page}.", page);
                throw new BlazorAPIException("Failed to search movies.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByDirector(string director, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(director)) throw new MovieValidationException("Search director name cannot be empty.");

            try
            {
                var searchPattern = $"%{director}%";

                var query = _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Director, searchPattern));

                var totalCount = await query.CountAsync();

                var movies = await query
                    .OrderBy(m => m.Director)
                    .ThenBy(m => m.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<MovieDTO>
                {
                    Items = MapMoviesToDtos(movies),
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by director for page {Page}.", page);
                throw new BlazorAPIException("Failed to search movies.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByGenre(string genre, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new MovieValidationException("Search genre cannot be empty.");

            try
            {
                var searchPattern = $"%{genre}%";

                var query = _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => EF.Functions.Like(m.Genre, searchPattern));

                var totalCount = await query.CountAsync();

                var movies = await query
                    .OrderBy(m => m.Genre)
                    .ThenBy(m => m.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<MovieDTO>
                {
                    Items = MapMoviesToDtos(movies),
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred searching movies by genre for page {Page}.", page);
                throw new BlazorAPIException("Failed to search movies.", 500);
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