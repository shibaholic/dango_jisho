using Application;
using Application.UseCaseCommands;
using Infrastructure;
using Infrastructure.DbContext;
using MediatR;
using Presentation;

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
        
        // APP
        var app = builder.Build();
    
        app.RunPresentationServices();
        app.CheckIfDatabaseCreated();
        await app.SeedData();
        
        Console.WriteLine("app.Run()");
        app.Run();
    }
}

public static class ApplicationBuilderExtensions
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
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));
                var request = new ImportJMdictRequest { Content = stream.ToArray() };
                var result = await mediator.Send(request, CancellationToken.None);
                if (!result.Successful)
                {
                    Console.WriteLine("Error while importing JMdict_e.xml.");
                    System.Environment.Exit(1);
                }
                Console.WriteLine($"{result.Message}");
            }
            else
            {
                Console.WriteLine("Database ensuring created...");
                dataContext.Database.EnsureCreated();
            }
                
        }
    }
}