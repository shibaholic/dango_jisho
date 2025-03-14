using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Presentation.Extensions;
using Presentation.Middleware;

namespace Presentation;

public static class ServiceExtension
{
    public static void ConfigurePresentationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Chuui API", Version = "v1" });
    
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
                // TODO: If development, use development Admin token
            });
    
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });

            options.CustomSchemaIds(type => type.ToString());
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 100_000_000;
        });

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.MaxRequestBodySize = 100_000_000;
        });
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder
                .WithOrigins("http://localhost:5010", "http://localhost:5173", "http://localhost:4173", "https://shibaholic.dev")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("fixed", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }));
        });
        
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                
                // var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                string? traceId = System.Diagnostics.Activity.Current?.TraceId.ToString();
                context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
            };
        });
        
        // JWT configuration
        builder.AddMySecretConfiguration();
        builder.AddJwtAuthentication();
    }

    public static void RunPresentationServices(this WebApplication app)
    {
        app.UseCors();
        
        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentName}/swagger.json"; // Sets the route for Swagger JSON
        });
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = "api/swagger";
        });
        // }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRateLimiter();

        app.UseMiddleware<TokenValidationMiddleware>();
        
        app.MapControllers();
    }
}