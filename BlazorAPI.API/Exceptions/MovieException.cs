namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Base exception class for Movie Manager application.
    /// All application-specific exceptions inherit from this class.
    /// </summary>
    public class MovieException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the MovieException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MovieException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MovieException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public MovieException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the MovieException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MovieException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
