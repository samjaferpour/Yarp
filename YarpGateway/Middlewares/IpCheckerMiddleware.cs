namespace YarpGateway.Middlewares
{
    public class IpCheckerMiddleware
    {
        private readonly RequestDelegate _next;

        public IpCheckerMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress == null ? string.Empty : context.Connection.RemoteIpAddress.ToString();

            // Validate IP
            if (string.IsNullOrEmpty(ip) || !IsValidIp(ip))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid IP Address");
                return;
            }

            // IP is valid
            await _next(context);
            //await context.Response.WriteAsync("Valid IP Address");
        }

        private bool IsValidIp(string ip)
        {
            // Example: Check if the IP is valid by comparing it against a list of IPs
            var validIps = new List<string> { "::1", "192.168.6.4", "192.168.6.8" };
            return validIps.Contains(ip);
        }
    }
    public static class IpCheckerMiddlewareExtension
    {
        public static void UseIpChecker(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<IpCheckerMiddleware>();
        }
    }
}
