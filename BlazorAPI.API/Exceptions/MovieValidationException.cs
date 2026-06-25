namespace BlazorAPI.API.Exceptions
{
    /// <summary>
    /// Exception thrown when movie data validation fails.
    /// </summary>
    public class MovieValidationException : BlazorAPIException
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
