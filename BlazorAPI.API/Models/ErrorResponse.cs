namespace BlazorAPI.API.Models
{
    // CHANGE: Extracted ErrorResponse out of ExceptionHandlingMiddleware.cs into its own file.
    //         A class and middleware living in the same file violates the Single Responsibility
    //         Principle and makes both harder to find and test independently.

    /// <summary>
    /// Represents the standard error payload returned by the API for all non-2xx responses.
    /// </summary>
    public sealed class ErrorResponse
    {
        // CHANGE: Marked class as 'sealed' — this is a plain data carrier and is not
        //         intended to be subclassed.

        /// <summary>HTTP status code (e.g. 400, 404, 500).</summary>
        public int StatusCode { get; init; }

        /// <summary>Short, human-readable status title (e.g. "Not Found").</summary>
        public string Title { get; init; } = "Error";

        /// <summary>Detailed error message safe to return to the caller.</summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>UTC timestamp of when the error occurred.</summary>
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        /// Field-level validation errors, present only on 400 Validation Error responses.
        /// Keys are field names; values are arrays of error messages for that field.
        /// </summary>
        public Dictionary<string, string[]>? ValidationErrors { get; init; }

        // CHANGE: Replaced { get; set; } with { get; init; } throughout.
        //         ErrorResponse should be immutable once constructed — there is no reason
        //         to mutate it after it has been built in the middleware.
    }
}
