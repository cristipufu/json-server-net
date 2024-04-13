using System.Text.Json;

namespace JsonServer
{
    public static class ErrorHandler
    {
        public static async Task HandleAsync(HttpContext context, Exception ex)
        {
            switch (ex)
            {
                case FileNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("File not found");
                    break;
                case JsonException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid JSON format");
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"Internal server error: {ex.Message}");
                    break;
            }
        }
    }
}