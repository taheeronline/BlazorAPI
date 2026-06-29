namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Base exception class for Movie Manager application.
    /// All application-specific exceptions inherit from this class.
    /// </summary>
    public class MovieExceptions : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the MovieException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MovieExceptions(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MovieException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public MovieExceptions(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the MovieException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MovieExceptions(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a requested movie is not found.
    /// </summary>
    public class MovieNotFoundException : MovieExceptions
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

    /// <summary>
    /// Exception thrown when movie data validation fails.
    /// </summary>
    public class MovieValidationException : MovieExceptions
    {
        /// <summary>
        /// Gets the validation errors associated with this exception.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the MovieValidationException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MovieValidationException(string message)
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the MovieValidationException class with validation errors.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errors">Dictionary of validation errors by field name.</param>
        public MovieValidationException(string message, Dictionary<string, string[]> errors)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
}
