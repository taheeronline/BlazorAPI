using BlazorAPI.API.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using BlazorAPI.API.Models;
using System.Net;
using System.Text.Json;

namespace BlazorAPI.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware. Catches all unhandled exceptions,
    /// maps them to structured HTTP responses, and logs them appropriately.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        // CHANGE: Made options a static field — JsonSerializerOptions is expensive to
        //         construct and should never be recreated per-request.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // CHANGE: Do not attempt to write a response if one has already started
                //         streaming to the client — doing so throws an InvalidOperationException.
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response already started; cannot write error response.");
                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // CHANGE: Added OperationCanceledException handling. When a client disconnects,
            //         ASP.NET Core throws this internally. Returning 499 (client closed request)
            //         avoids logging it as an unexpected 500 error.
            var (statusCode, message, validationErrors) = exception switch
            {
                OperationCanceledException =>
                    (499, "Request was cancelled by the client.", null),

                MovieNotFoundException =>
                    (StatusCodes.Status404NotFound, exception.Message, (Dictionary<string, string[]>?)null),

                MovieValidationException ex =>
                    (StatusCodes.Status400BadRequest, ex.Message, ex.Errors),

                MovieExceptions ex =>
                    (ex.StatusCode, ex.Message, (Dictionary<string, string[]>?)null),

                ArgumentNullException =>
                    (StatusCodes.Status400BadRequest, exception.Message, (Dictionary<string, string[]>?)null),

                ArgumentException =>
                    (StatusCodes.Status400BadRequest, exception.Message, (Dictionary<string, string[]>?)null),

                // Default: treat as 500, do not leak internal details to the client.
                _ => (StatusCodes.Status500InternalServerError,
                      "An unexpected error occurred. Please try again later.",
                      (Dictionary<string, string[]>?)null)
            };

            // CHANGE: Log 499s at Debug level — they are not errors.
            //         Log 5xx at Error level with full stack trace.
            //         Log 4xx at Warning level — expected business logic failures.
            if (statusCode == 499)
                _logger.LogDebug("Request cancelled by client.");
            else if (statusCode >= 500)
                _logger.LogError(exception, "Unhandled exception — HTTP {StatusCode}.", statusCode);
            else
                _logger.LogWarning("Handled exception — HTTP {StatusCode}: {Message}", statusCode, message);

            // CHANGE: Replaced the custom GetExceptionTitle() helper with
            //         ReasonPhrases.GetReasonPhrase() from Microsoft.AspNetCore.WebUtilities.
            //         This is the authoritative source and handles all standard status codes
            //         without a hand-rolled switch statement.
            var title = statusCode == 499
                ? "Client Closed Request"
                : ReasonPhrases.GetReasonPhrase(statusCode);

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Title = title,
                Message = message,
                Timestamp = DateTimeOffset.UtcNow,
                ValidationErrors = validationErrors
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}