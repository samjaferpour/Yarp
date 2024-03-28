namespace YarpGateway.Middlewares
{
    public class ApiKeyCheckerMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyCheckerMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var apiKey = context.Request.Headers["ApiKey"].FirstOrDefault();

            // Validate API Key
            if (string.IsNullOrEmpty(apiKey) || !IsValidApiKey(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            // API Key is valid
            await _next(context);
            //await context.Response.WriteAsync("Valid API Key");
        }

        private bool IsValidApiKey(string apiKey)
        {
            // Example: Check if the API key is valid by comparing it against a list of valid keys
            var validApiKeys = new List<string> { "your-api-key1", "your-api-key2" };
            return validApiKeys.Contains(apiKey);
        }
    }
    public static class ApiKeyCheckerMiddlewareExtension
    {
        public static void UseApiKeyChecker(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ApiKeyCheckerMiddleware>();
        }
    }
}
