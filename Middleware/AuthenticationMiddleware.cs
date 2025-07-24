public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiToken = "YourSecureToken123"; // Replace with env var in prod

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var token) || token != $"Bearer {ApiToken}")
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await _next(context);
    }
}
