namespace MovieManager.API.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested movie is not found.
    /// </summary>
    public class MovieNotFoundException : MovieManagerException
    {
        /// <summary>
        /// Initializes a new instance of the MovieNotFoundException class.
        /// </summary>
        /// <param name="movieId">The ID of the movie that was not found.</param>
        public MovieNotFoundException(int movieId)
            : base($"Movie with ID '{movieId}' was not found. Please verify the ID and try again.", 404)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MovieNotFoundException class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MovieNotFoundException(string message)
            : base(message, 404)
        {
        }
    }
}
