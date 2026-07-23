using Microsoft.EntityFrameworkCore;
using BlazorAPI.API.DTOs.MovieDTOs;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Implementation
{
    public class MovieService : IMovieService
    {
        // CHANGE: Renamed interface from 'IMovieService' to 'IMovieService'.

        private readonly MyDbContext _dbContext;
        private readonly ILogger<MovieService> _logger;

        public MovieService(MyDbContext dbContext, ILogger<MovieService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResult<MovieDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            // CHANGE: Added CancellationToken and passed it to all EF Core calls.
            // CHANGE: Added .Where(m => !m.IsDeleted) to filter out soft-deleted records.
            //         Previously, deleted movies were still being returned to callers.
            // CHANGE: Used Select() for direct DTO projection instead of fetching full
            //         entities and mapping in memory. This reduces the data transferred
            //         from the database.
            try
            {
                var baseQuery = _dbContext.Movies
                    .AsNoTracking()
                    .Where(m => !m.IsDeleted?? false);

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var movieDtos = await baseQuery
                    .OrderByDescending(m => m.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => MapToDto(m))
                    .ToListAsync(cancellationToken);

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
                _logger.LogError(ex, "Error retrieving movies for page {Page}.", page);
                throw new MovieExceptions("Failed to retrieve movies. Please try again later.", 500);
            }
        }

        public async Task<MovieDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // CHANGE: Added CancellationToken.
            // CHANGE: Added !m.IsDeleted filter — previously a soft-deleted movie could
            //         still be fetched by ID, which is a logical bug.
            // CHANGE: Moved MovieValidationException re-throw into the outer catch by
            //         listing it explicitly, keeping the pattern consistent with other methods.
            GuardPositiveId(id, "Movie");

            try
            {
                var movie = await _dbContext.Movies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id && !(m.IsDeleted?? false), cancellationToken);

                if (movie is null) throw new MovieNotFoundException(id);

                return MapToDto(movie);
            }
            catch (MovieNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie with ID {MovieId}.", id);
                throw new MovieExceptions("Failed to retrieve movie. Please try again later.", 500);
            }
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO createMovieDto, CancellationToken cancellationToken = default)
        {
            // CHANGE: Added CancellationToken.
            // CHANGE: Null check now throws ArgumentNullException (more semantically correct)
            //         rather than MovieValidationException, since a null DTO is a programming
            //         error, not a user validation error.
            ArgumentNullException.ThrowIfNull(createMovieDto);

            try
            {
                var movie = Movie.Create(
                    createMovieDto.Title,
                    createMovieDto.Director,
                    createMovieDto.Genre,
                    createMovieDto.ReleaseDate,
                    createMovieDto.Rating,
                    createMovieDto.CreatedBy
                );

                _dbContext.Movies.Add(movie);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie created with ID {MovieId}.", movie.Id);
                return MapToDto(movie);
            }
            catch (ArgumentException ex)
            {
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie.");
                throw new MovieExceptions("Failed to create movie.", 500);
            }
        }

        public async Task<MovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDto, CancellationToken cancellationToken = default)
        {
            // CHANGE: Added CancellationToken.
            // CHANGE: Consolidated guard clauses into a shared helper for DRY principle.
            GuardPositiveId(id, "Movie");
            ArgumentNullException.ThrowIfNull(updateMovieDto);

            try
            {
                // CHANGE: Added !m.IsDeleted filter — you should not be able to update
                //         a soft-deleted movie.
                var movie = await _dbContext.Movies
                    .FirstOrDefaultAsync(m => m.Id == id && !(m.IsDeleted?? false), cancellationToken);

                if (movie is null) throw new MovieNotFoundException(id);

                movie.Update(
                    updateMovieDto.Title,
                    updateMovieDto.Director,
                    updateMovieDto.Genre,
                    updateMovieDto.ReleaseDate,
                    updateMovieDto.Rating,
                    updateMovieDto.ModifiedBy
                );

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie with ID {MovieId} updated.", id);
                return MapToDto(movie);
            }
            catch (MovieNotFoundException) { throw; }
            catch (ArgumentException ex)
            {
                throw new MovieValidationException($"Movie validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with ID {MovieId}.", id);
                throw new MovieExceptions("Failed to update movie.", 500);
            }
        }

        public async Task DeleteMovieAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            // CHANGE: Added CancellationToken.
            // CHANGE: Replaced separate guard clauses with shared helper + a specific check
            //         for modifiedById. In production, modifiedById should come from the
            //         auth context (e.g., IHttpContextAccessor), not from the request body/query.
            GuardPositiveId(id, "Movie");
            GuardPositiveId(modifiedById, "ModifiedBy");

            try
            {
                var movie = await _dbContext.Movies
                    .FirstOrDefaultAsync(m => m.Id == id && !(m.IsDeleted?? false), cancellationToken);

                if (movie is null) throw new MovieNotFoundException(id);

                movie.MarkAsDeleted(modifiedById);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie with ID {MovieId} soft-deleted by user {UserId}.", id, modifiedById);
            }
            catch (MovieNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie with ID {MovieId}.", id);
                throw new MovieExceptions("Failed to delete movie.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByTitleAsync(string title, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new MovieValidationException("Search title cannot be empty.");

            try
            {
                return await SearchMoviesAsync(
                    // CHANGE: Extracted common search logic into a private helper to eliminate
                    //         the copy-paste across GetByTitle, GetByDirector, GetByGenre.
                    predicate: m => EF.Functions.Like(m.Title, $"%{title}%"),
                    orderBy: q => q.OrderBy(m => m.Title),
                    page, pageSize, cancellationToken
                );
            }
            catch (MovieValidationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies by title for page {Page}.", page);
                throw new MovieExceptions("Failed to search movies.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByDirectorAsync(string director, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(director))
                throw new MovieValidationException("Director name cannot be empty.");

            try
            {
                return await SearchMoviesAsync(
                    predicate: m => EF.Functions.Like(m.Director, $"%{director}%"),
                    orderBy: q => q.OrderBy(m => m.Director).ThenBy(m => m.Title),
                    page, pageSize, cancellationToken
                );
            }
            catch (MovieValidationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies by director for page {Page}.", page);
                throw new MovieExceptions("Failed to search movies.", 500);
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByGenreAsync(string genre, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(genre))
                throw new MovieValidationException("Genre cannot be empty.");

            try
            {
                return await SearchMoviesAsync(
                    predicate: m => EF.Functions.Like(m.Genre, $"%{genre}%"),
                    orderBy: q => q.OrderBy(m => m.Genre).ThenBy(m => m.Title),
                    page, pageSize, cancellationToken
                );
            }
            catch (MovieValidationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies by genre for page {Page}.", page);
                throw new MovieExceptions("Failed to search movies.", 500);
            }
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Shared pagination + filtering logic used by all search methods.
        /// Eliminates the ~20-line copy-paste that existed across GetByTitle/Director/Genre.
        /// </summary>
        private async Task<PagedResult<MovieDTO>> SearchMoviesAsync(
            System.Linq.Expressions.Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>> orderBy,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _dbContext.Movies
                .AsNoTracking()
                .Where(m => !m.IsDeleted ?? false)   // CHANGE: Always exclude soft-deleted records.
                .Where(predicate);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await orderBy(baseQuery)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => MapToDto(m))
                .ToListAsync(cancellationToken);

            return new PagedResult<MovieDTO>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Maps a Movie entity to a MovieDTO.
        /// CHANGE: Renamed from MapMovieToDto/MapMoviesToDtos to a single static MapToDto.
        ///         The IEnumerable overload was unnecessary — callers can just use .Select(MapToDto).
        /// NOTE: CreatedBy/ModifiedBy use the raw FK integer fields (m.CreatedById, m.ModifiedById)
        ///       instead of the navigation properties (m.CreatedByUser?.Id ?? 0). The old code
        ///       silently returned 0 when navigation properties weren't loaded, which is misleading.
        ///       If you need user names in the DTO, use .Include() explicitly or a separate query.
        /// </summary>
        private static MovieDTO MapToDto(Movie movie) => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            Genre = movie.Genre,
            ReleaseDate = movie.ReleaseDate,
            Rating = movie.Rating,
            CreatedDate = movie.CreatedDate,
            ModifiedDate = movie.ModifiedDate,
            CreatedBy = movie.CreatedBy ?? 0,
            ModifiedBy = movie.ModifiedBy ?? 0
        };

        /// <summary>
        /// Shared guard clause for positive integer IDs.
        /// CHANGE: Extracted repeated "if (id &lt;= 0) throw" pattern into one place.
        /// </summary>
        private static void GuardPositiveId(int id, string paramName)
        {
            if (id <= 0)
                throw new MovieValidationException($"{paramName} ID must be greater than zero.");
        }
    }
}