using Microsoft.AspNetCore.Mvc;
using BlazorAPI.API.DTOs.MovieDTOs;
using BlazorAPI.API.Exceptions; // Required for catching custom exceptions
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Controllers
{
    /// <summary>
    /// API controller for managing movies.
    /// Provides endpoints for CRUD operations and searching movies.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a paginated list of all non-deleted movies.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<MovieDTO>>> GetAllMovies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET all movies — Page: {Page}, Size: {PageSize}", page, pageSize);
            var result = await _movieService.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a specific movie by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> GetMovieById(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET movie by ID {MovieId}", id);

            try
            {
                var movie = await _movieService.GetByIdAsync(id, cancellationToken);
                return Ok(movie);
            }
            catch (MovieNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new movie.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> CreateMovie(
            [FromBody] CreateMovieDTO createMovieDto,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST create movie");

            try
            {
                var created = await _movieService.CreateMovieAsync(createMovieDto, cancellationToken);
                return CreatedAtAction(nameof(GetMovieById), new { id = created.Id }, created);
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing movie.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> UpdateMovie(
            int id,
            [FromBody] UpdateMovieDTO updateMovieDto,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT update movie ID {MovieId}", id);

            try
            {
                var updated = await _movieService.UpdateMovieAsync(id, updateMovieDto, cancellationToken);
                return Ok(updated);
            }
            catch (MovieNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Soft-deletes a movie.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie(
            int id,
            [FromQuery] int modifiedById,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE movie ID {MovieId} by user {UserId}", id, modifiedById);

            try
            {
                await _movieService.DeleteMovieAsync(id, modifiedById, cancellationToken);
                return NoContent();
            }
            catch (MovieNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------------------
        // Search endpoints
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Searches movies by title (partial, case-insensitive).
        /// </summary>
        [HttpGet("search/title/{title}")]
        [ProducesResponseType(typeof(PagedResult<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<MovieDTO>>> SearchByTitle(
            string title,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Search by title '{Title}' — Page {Page}", title, page);

            try
            {
                var result = await _movieService.GetByTitleAsync(title, page, pageSize, cancellationToken);
                return Ok(result);
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Searches movies by director name (partial, case-insensitive).
        /// </summary>
        [HttpGet("search/director/{director}")]
        [ProducesResponseType(typeof(PagedResult<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<MovieDTO>>> SearchByDirector(
            string director,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Search by director '{Director}' — Page {Page}", director, page);

            try
            {
                var result = await _movieService.GetByDirectorAsync(director, page, pageSize, cancellationToken);
                return Ok(result);
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Searches movies by genre (partial, case-insensitive).
        /// </summary>
        [HttpGet("search/genre/{genre}")]
        [ProducesResponseType(typeof(PagedResult<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<MovieDTO>>> SearchByGenre(
            string genre,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Search by genre '{Genre}' — Page {Page}", genre, page);

            try
            {
                var result = await _movieService.GetByGenreAsync(genre, page, pageSize, cancellationToken);
                return Ok(result);
            }
            catch (MovieValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}