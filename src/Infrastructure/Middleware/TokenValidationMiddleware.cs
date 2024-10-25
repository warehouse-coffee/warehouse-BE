using Microsoft.AspNetCore.Http;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Infrastructure.Identity;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IIdentityService identityService)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (context.Request.Path.StartsWithSegments("/api/IdentityUser/signin") ||
       context.Request.Path.Equals("/api/index.html"))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrEmpty(token) || !await identityService.ValidateTokenAsync(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}
