namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Base exception class for the Book module.
    /// All book-specific exceptions inherit from this class.
    /// </summary>
    public class BookException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the BookException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BookException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BookException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public BookException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the BookException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BookException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a requested book is not found.
    /// </summary>
    public class BookNotFoundException : BookException
    {
        /// <summary>
        /// Initializes a new instance of the BookNotFoundException class.
        /// </summary>
        /// <param name="bookId">The ID of the book that was not found.</param>
        public BookNotFoundException(int bookId)
            : base($"Book with ID '{bookId}' was not found. Please verify the ID and try again.", 404)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BookNotFoundException class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BookNotFoundException(string message)
            : base(message, 404)
        {
        }
    }

    /// <summary>
    /// Exception thrown when book data validation fails.
    /// </summary>
    public class BookValidationException : BookException
    {
        /// <summary>
        /// Gets the validation errors associated with this exception.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the BookValidationException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public BookValidationException(string message)
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the BookValidationException class with validation errors.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errors">Dictionary of validation errors by field name.</param>
        public BookValidationException(string message, Dictionary<string, string[]> errors)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
}
