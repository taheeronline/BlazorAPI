using MovieManager.API.DTOs;

namespace MovieManager.API.Services
{
    /// <summary>
    /// Service interface for Movie-related operations.
    /// Provides CRUD operations and search functionality.
    /// </summary>
    public interface iMovieService
    {
        /// <summary>
        /// Retrieves all movies from the database.
        /// </summary>
        /// <returns>Collection of all movies.</returns>
        Task<IEnumerable<MovieDTO>> GetAllMoviesAsync();

        /// <summary>
        /// Retrieves a movie by its unique identifier.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>Movie data if found. Throws MovieNotFoundException (404) if not found.</returns>
        Task<MovieDTO> GetMovieByIdAsync(int id);

        /// <summary>
        /// Creates a new movie in the database.
        /// </summary>
        /// <param name="createMovieDto">The movie data to create.</param>
        /// <returns>The created movie with assigned Id.</returns>
        Task<MovieDTO> CreateMovieAsync(CreateMovieDTO createMovieDto);

        /// <summary>
        /// Updates an existing movie in the database.
        /// </summary>
        /// <param name="id">The movie identifier to update.</param>
        /// <param name="updateMovieDto">The updated movie data.</param>
        /// <returns>The updated movie data.</returns>
        Task<MovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDto);

        /// <summary>
        /// Deletes a movie from the database by its identifier (Soft Delete).
        /// </summary>
        /// <param name="id">The movie identifier to delete.</param>
        Task DeleteMovieAsync(int id);

        /// <summary>
        /// Searches for movies by title (case-insensitive partial match).
        /// </summary>
        /// <param name="title">The movie title to search for.</param>
        /// <returns>Collection of movies matching the title search.</returns>
        Task<IEnumerable<MovieDTO>> SearchMoviesByTitleAsync(string title);

        /// <summary>
        /// Searches for movies by director name (case-insensitive partial match).
        /// </summary>
        /// <param name="director">The director name to search for.</param>
        /// <returns>Collection of movies matching the director search.</returns>
        Task<IEnumerable<MovieDTO>> SearchMoviesByDirectorAsync(string director);

        /// <summary>
        /// Searches for movies by genre (case-insensitive partial match).
        /// </summary>
        /// <param name="genre">The genre to search for.</param>
        /// <returns>Collection of movies matching the genre search.</returns>
        Task<IEnumerable<MovieDTO>> SearchMoviesByGenreAsync(string genre);
    }
}