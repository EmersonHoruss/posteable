using Postable.Entities.Dtos;

namespace Postable.Middelwares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    code = 401,
                    error = "Unauthorized",
                    message = "Unauthorized. Please, give a valid token.",
                    timestamp = DateTime.UtcNow
                });
            }
            else if (context.Response.StatusCode == 403)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    code = 403,
                    error = "Forbidden",
                    message = "Forbidden. No access for requested resource.",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
