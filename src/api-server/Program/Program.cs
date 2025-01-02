using System.Diagnostics;
using Application;
using Application.UseCaseCommands;
using Infrastructure;
using Infrastructure.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Presentation;
using Presentation.ExceptionHandler;

namespace Program;

public class Program
{
    public static async Task Main(string[] args)
    {
        // BUILD
        var builder = WebApplication.CreateBuilder(args);
        
        builder.ConfigurePresentationServices();
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices(builder.Configuration);

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

        builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
        
        // APP
        var app = builder.Build();
    
        app.RunPresentationServices();
        app.CheckIfDatabaseCreated();
        app.SeedData();

        app.UseExceptionHandler();
        
        Console.WriteLine("app.Run()");
        app.Run();
    }
}

public static class ApplicationExtensions
{
    public static void CheckIfDatabaseCreated(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
    
        if (dataContext!.CheckConnection())
        {
            Console.WriteLine("Database connected.");
        }
        else
        {
            Console.Error.WriteLine("Could not connect to database.");
            System.Environment.Exit(1);
        }
    }

    public static async Task SeedData(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
        var mediator = serviceScope.ServiceProvider.GetService<IMediator>();
        
        if (app.Environment.IsDevelopment())
        {
            var deleteCreate = (string)app.Configuration.GetValue(typeof(string), "EFCORE_DELETECREATE");
            if (deleteCreate == "true")
            {
                Console.WriteLine("Database ensuring deleted and created...");
                dataContext.Database.EnsureDeleted();
                dataContext.Database.EnsureCreated();
                    
                Console.WriteLine("Seeding with JMDict data...");
                string filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "JMdict_e.xml");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Could not find seed data.");
                    System.Environment.Exit(1);
                }
                var stopwatch = Stopwatch.StartNew();
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));
                var request = new ImportJMdictRequest { Content = stream.ToArray() };
                stopwatch.Stop();
                Console.WriteLine($"  Loading JMdict from {filePath} took {stopwatch.ElapsedMilliseconds} ms.");
                stopwatch.Restart();
                var result = await mediator.Send(request, CancellationToken.None);
                
                if (!result.Successful)
                {
                    Console.WriteLine("Error while importing JMdict_e.xml.");
                    System.Environment.Exit(1);
                }
                Console.WriteLine($"{result.Message}");
                stopwatch.Stop();
                Console.WriteLine($"  ImportJMdict took {stopwatch.ElapsedMilliseconds} ms.");
            }
            else
            {
                Console.WriteLine("Database ensuring created...");
                dataContext.Database.EnsureCreated();
            }
            
        }
    }
}