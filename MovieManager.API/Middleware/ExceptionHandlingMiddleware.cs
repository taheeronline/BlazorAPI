using System.Net;
using System.Text.Json;
using MovieManager.API.Exceptions;

namespace MovieManager.API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally across the application.
    /// Converts exceptions to appropriate HTTP responses with readable error messages.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the ExceptionHandlingMiddleware class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">Logger for recording exception details.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request and handle exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
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

        /// <summary>
        /// Handles the exception and returns an appropriate HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="exception">The exception that was thrown.</param>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = exception.Message,
                Timestamp = DateTimeOffset.UtcNow
            };

            // Handle specific application exceptions
            if (exception is MovieNotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.StatusCode = 404;
                response.Title = "Not Found";
            }
            else if (exception is MovieValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = 400;
                response.Title = "Validation Error";
                response.ValidationErrors = validationException.Errors;
            }
            else if (exception is MovieManagerException movieException)
            {
                context.Response.StatusCode = movieException.StatusCode;
                response.StatusCode = movieException.StatusCode;
                response.Title = GetExceptionTitle(movieException.StatusCode);
            }
            else if (exception is ArgumentException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = 400;
                response.Title = "Invalid Argument";
            }
            else
            {
                // Generic unhandled exceptions
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.StatusCode = 500;
                response.Title = "Internal Server Error";
                response.Message = "An unexpected error occurred. Please try again later.";
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Gets the appropriate title/description for an HTTP status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A human-readable title for the status code.</returns>
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

    /// <summary>
    /// Response model for error responses sent to clients.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// HTTP status code of the error.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Title/type of the error.
        /// </summary>
        public string Title { get; set; } = "Error";

        /// <summary>
        /// Detailed error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the error occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Validation errors, if applicable.
        /// </summary>
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }
}
