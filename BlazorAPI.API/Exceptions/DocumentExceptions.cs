namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Base exception class for the Document module.
    /// All document-specific exceptions inherit from this class.
    /// </summary>
    public class DocumentExceptions : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the DocumentException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DocumentExceptions(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DocumentException class with a specified HTTP status code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with this exception.</param>
        public DocumentExceptions(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the DocumentException class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DocumentExceptions(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a requested document is not found.
    /// </summary>
    public class DocumentNotFoundException : DocumentExceptions
    {
        /// <summary>
        /// Initializes a new instance of the DocumentNotFoundException class.
        /// </summary>
        /// <param name="documentId">The ID of the document that was not found.</param>
        public DocumentNotFoundException(int documentId)
            : base($"Document with ID '{documentId}' was not found. Please verify the ID and try again.", 404)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DocumentNotFoundException class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DocumentNotFoundException(string message)
            : base(message, 404)
        {
        }
    }

    /// <summary>
    /// Exception thrown when document data validation fails.
    /// </summary>
    public class DocumentValidationException : DocumentExceptions
    {
        /// <summary>
        /// Gets the validation errors associated with this exception.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the DocumentValidationException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DocumentValidationException(string message)
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentValidationException class with validation errors.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errors">Dictionary of validation errors by field name.</param>
        public DocumentValidationException(string message, Dictionary<string, string[]> errors)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
}