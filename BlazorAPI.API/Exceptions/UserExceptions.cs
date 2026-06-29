namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Base exception class for the User module.
    /// All user-specific exceptions inherit from this class.
    /// </summary>
    public class UserException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the UserException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public UserException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the UserException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a requested user is not found.
    /// </summary>
    public class UserNotFoundException : UserException
    {
        /// <summary>
        /// Initializes a new instance of the UserNotFoundException class.
        /// </summary>
        /// <param name="userId">The ID of the user that was not found.</param>
        public UserNotFoundException(int userId)
            : base($"User with ID '{userId}' was not found. Please verify the ID and try again.", 404)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserNotFoundException class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserNotFoundException(string message)
            : base(message, 404)
        {
        }
    }

    /// <summary>
    /// Exception thrown when user data validation fails.
    /// </summary>
    public class UserValidationException : UserException
    {
        /// <summary>
        /// Gets the validation errors associated with this exception.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the UserValidationException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserValidationException(string message)
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the UserValidationException class with validation errors.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errors">Dictionary of validation errors by field name.</param>
        public UserValidationException(string message, Dictionary<string, string[]> errors)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
}
