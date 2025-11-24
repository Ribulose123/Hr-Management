using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Middleware
{
    public class ErrorHandelingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandelingMiddleware> _logger;

        public ErrorHandelingMiddleware(RequestDelegate next, ILogger<ErrorHandelingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // let the pipeline continue
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred.");
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(new { error = "Data conflict. Please try again." });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "A database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
