using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Mappings.EntityDtos;
using Domain.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Extensions;

public static class JwtExtension
{
    // extension method for WebApplicationBuilder
    public static void AddMySecretConfiguration(this WebApplicationBuilder builder)
    {
        var jwtPrivateKey = builder.Configuration["Secrets:JwtPrivateKey"];
        if (jwtPrivateKey == null) Console.Error.WriteLine("No private key found");
        MySecretConfiguration.Secrets.JwtPrivateKey = jwtPrivateKey ?? String.Empty;
        if (builder.Environment.IsDevelopment())
        {
            MySecretConfiguration.Secrets.Issuer = "http://localhost:5010";
            MySecretConfiguration.Secrets.Audience = "http://localhost:5173";
        }
        else
        {
            var domain = "https://shibaholic.dev";
            MySecretConfiguration.Secrets.Issuer = domain;
            MySecretConfiguration.Secrets.Audience = domain;
        }
    }
    
    // extension method for WebApplicationBuilder
    public static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(MySecretConfiguration.Secrets.JwtPrivateKey)),
                    ValidateIssuer = true,
                    ValidIssuer = MySecretConfiguration.Secrets.Issuer,
                    ValidateAudience = true,
                    ValidAudience = MySecretConfiguration.Secrets.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // x.Authority = "Authority URL"; // replace with URL?
                
                // WebSocket access token is in the query string due to a limitation in Browser APIs.
                // Here it is restricted to only calls to the SignalR hub.
                // Note that when using HTTPS, query string values are secured by TLS connection. However, many servers log query string values.
                // For more security considerations, please read security docs: https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Request.Cookies.TryGetValue("accessToken", out var accessToken); 

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            //read the token of the query string
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    // OnAuthenticationFailed = context =>
                    // {
                    //     Console.WriteLine("\nAuthentication Failed");
                    //     
                    //     if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    //     {
                    //         Console.WriteLine("  ExpiredException");
                    //         context.Response.Headers.Add("Token-Expired", "true");
                    //         context.Response.StatusCode = 401;
                    //         return Task.CompletedTask;
                    //     }
                    //     return Task.CompletedTask;
                    // }
                };
            });
        builder.Services.AddAuthorization();
    }

    public static void SetTokensInsideCookie(string? accessToken, Guid? refreshToken, HttpContext context)
    {
        if (!accessToken.IsNullOrEmpty())
        {
            context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
        else
        {
            context.Response.Cookies.Delete("accessToken");
        }
        
        if (refreshToken.HasValue)
        {
            context.Response.Cookies.Append("refreshToken", refreshToken!.Value.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
        else
        {
            context.Response.Cookies.Delete("refreshToken");
        }
    }
    
    public static string Generate(UserAuthDto data)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(MySecretConfiguration.Secrets.JwtPrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = MySecretConfiguration.Secrets.Issuer,
            Audience = MySecretConfiguration.Secrets.Audience,
            Subject = GenerateClaims(data),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(UserAuthDto userAuth)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim("Id", userAuth.User.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, userAuth.User.Username));
        ci.AddClaim(new Claim(ClaimTypes.Role, "User"));
        if(userAuth.User.IsAdmin) ci.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

        return ci;
    }
}