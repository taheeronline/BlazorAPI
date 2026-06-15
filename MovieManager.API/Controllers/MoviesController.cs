using Microsoft.AspNetCore.Mvc;
using MovieManager.API.DTOs.MovieDTOs;
using MovieManager.API.Services;

namespace MovieManager.API.Controllers
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
        private readonly iMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        /// <summary>
        /// Initializes a new instance of the MoviesController class.
        /// </summary>
        /// <param name="movieService">The movie service for handling business logic.</param>
        /// <param name="logger">Logger for recording controller actions.</param>
        public MoviesController(iMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all movies from the database.
        /// </summary>
        /// <returns>A collection of all movies.</returns>
        /// <response code="200">Returns the list of all movies.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAllMovies()
        {
            _logger.LogInformation("GET request: Retrieve all movies");

            var movies = await _movieService.GetAll();
            return Ok(movies);
        }

        /// <summary>
        /// Creates a new movie in the database.
        /// </summary>
        /// <param name="createMovieDto">The movie data to create.</param>
        /// <returns>The created movie with its assigned ID.</returns>
        /// <response code="201">Movie successfully created.</response>
        /// <response code="400">Invalid movie data provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> CreateMovie([FromBody] CreateMovieDTO createMovieDto)
        {
            _logger.LogInformation("POST request: Create new movie");

            var createdMovie = await _movieService.CreateMovie(createMovieDto);
            return CreatedAtAction(nameof(GetMovieById), new { id = createdMovie.Id }, createdMovie);
        }

        /// <summary>
        /// Updates an existing movie in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the movie to update.</param>
        /// <param name="updateMovieDto">The updated movie data.</param>
        /// <returns>The updated movie.</returns>
        /// <response code="200">Movie successfully updated.</response>
        /// <response code="400">Invalid movie data provided.</response>
        /// <response code="404">Movie not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> UpdateMovie(int id, [FromBody] UpdateMovieDTO updateMovieDto)
        {
            _logger.LogInformation("PUT request: Update movie with ID {movieId}", id);

            var updatedMovie = await _movieService.UpdateMovie(id, updateMovieDto);
            return Ok(updatedMovie);
        }

        /// <summary>
        /// Deletes a movie from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the movie to delete.</param>
        /// <returns>No content response on successful deletion.</returns>
        /// <response code="204">Movie successfully deleted.</response>
        /// <response code="404">Movie not found.</response>
        /// <response code="400">Invalid movie ID provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _logger.LogInformation("DELETE request: Delete movie with ID {movieId}", id);

            await _movieService.DeleteMovie(id);
            return NoContent();
        }

        /// <summary>
        /// Gets a specific movie by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the movie.</param>
        /// <returns>The movie with the specified ID.</returns>
        /// <response code="200">Returns the requested movie.</response>
        /// <response code="404">Movie not found.</response>
        /// <response code="400">Invalid movie ID provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieDTO>> GetMovieById(int id)
        {
            _logger.LogInformation("GET request: Retrieve movie with ID {movieId}", id);

            var movie = await _movieService.GetById(id);
            return Ok(movie);
        }

        /// <summary>
        /// Searches for movies by title (case-insensitive).
        /// </summary>
        /// <param name="title">The movie title or partial title to search for.</param>
        /// <returns>A collection of movies matching the search title.</returns>
        /// <response code="200">Returns the list of movies matching the search criteria.</response>
        /// <response code="400">Invalid search title provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("search/title/{title}")]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> SearchByTitle(string title)
        {
            _logger.LogInformation("GET request: Search movies by title '{title}'", title);

            var movies = await _movieService.GetByTitle(title);
            return Ok(movies);
        }

        /// <summary>
        /// Searches for movies by director name (case-insensitive).
        /// </summary>
        /// <param name="director">The director name to search for.</param>
        /// <returns>A collection of movies by the specified director.</returns>
        /// <response code="200">Returns the list of movies by the specified director.</response>
        /// <response code="400">Invalid director name provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("search/director/{director}")]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> SearchByDirector(string director)
        {
            _logger.LogInformation("GET request: Search movies by director '{director}'", director);

            var movies = await _movieService.GetByDirector(director);
            return Ok(movies);
        }

        /// <summary>
        /// Searches for movies by genre (case-insensitive).
        /// </summary>
        /// <param name="genre">The genre to search for.</param>
        /// <returns>A collection of movies in the specified genre.</returns>
        /// <response code="200">Returns the list of movies in the specified genre.</response>
        /// <response code="400">Invalid genre provided.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("search/genre/{genre}")]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> SearchByGenre(string genre)
        {
            _logger.LogInformation("GET request: Search movies by genre '{genre}'", genre);

            var movies = await _movieService.GetByGenre(genre);
            return Ok(movies);
        }
    }
}