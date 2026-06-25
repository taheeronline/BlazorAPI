using System.Text.Json;
using BlazorAPI.API.Exceptions;

namespace BlazorAPI.API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally across the application.
    /// Converts exceptions to appropriate HTTP responses with readable error messages.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // Removed 'static' so we can use _logger
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // 1. Map the exception to the correct status code and details using pattern matching
            var (statusCode, title, message, validationErrors) = exception switch
            {
                MovieNotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message, null),
                MovieValidationException ex => (StatusCodes.Status400BadRequest, "Validation Error", ex.Message, ex.Errors),
                MovieException ex => (ex.StatusCode, GetExceptionTitle(ex.StatusCode), ex.Message, null),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument", exception.Message, null),

                // Default fallback for unhandled errors
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.", null)
            };

            // 2. Log the exception
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                // CRITICAL: Log full stack traces for unexpected 500 errors
                _logger.LogError(exception, "An unhandled exception occurred processing the request.");
            }
            else
            {
                // Optional: Log handled business logic errors as warnings or information
                _logger.LogWarning("Handled exception ({StatusCode}): {Message}", statusCode, message);
            }

            // 3. Build the response
            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Title = title,
                Message = message,
                Timestamp = DateTimeOffset.UtcNow,
                ValidationErrors = validationErrors
            };

            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private static string GetExceptionTitle(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                404 => "Not Found",
                409 => "Conflict",
                500 => "Internal Server Error",
                _ => "Error"
            };
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; } = "Error";
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }
}