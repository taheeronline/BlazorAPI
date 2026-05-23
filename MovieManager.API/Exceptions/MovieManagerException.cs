namespace MovieManager.API.Exceptions
{
    /// <summary>
    /// Base exception class for Movie Manager application.
    /// All application-specific exceptions inherit from this class.
    /// </summary>
    public class MovieManagerException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the MovieManagerException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MovieManagerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MovieManagerException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public MovieManagerException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the MovieManagerException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MovieManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
