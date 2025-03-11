using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Presentation.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        // Console.WriteLine("\n");
        var token = context.Request.Cookies["accessToken"];
        var route = context.Request.Path.ToString();
        // Console.WriteLine($"route: {route}");
        if (route is "/api/auth/refresh" or "/api/auth/authenticate")
        {
            
        } else if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            // Console.WriteLine($"expClaim: {expClaim}");
            
            if (expClaim != null && long.TryParse(expClaim, out long exp))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                // Console.WriteLine($"expirationTime: {expirationTime}");
                // Console.WriteLine($"dateTime.UtcNow: {DateTime.UtcNow}");
                if (expirationTime < DateTime.UtcNow)
                {
                    // Console.WriteLine("ValidationMiddlware: Token is expired");
                    context.Response.StatusCode = 401;
                    context.Response.Cookies.Delete("accessToken");
                    await context.Response.WriteAsync("Token expired");
                    return;
                }
            }
        }

        await _next(context);
    }
}