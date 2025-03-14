using Application;
using Infrastructure;
using Infrastructure.DbContext;
using Presentation;
using Presentation.ExceptionHandler;
using Program.SeedService;

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

        builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
        
        // APP
        var app = builder.Build();
        
        app.RunPresentationServices();
        app.CheckIfDatabaseCreated();
        
        // TODO: make CheckEnsureCreated run in background
        await app.CheckEnsureCreated();

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
            // Console.WriteLine("PostgreSQL: " + app.Configuration.GetConnectionString("PostgreSQL"));
            // Console.WriteLine("PrivateKey: " + app.Configuration.GetSection("Secrets:JwtPrivateKey").Value);
            System.Environment.Exit(1);
        }
    }

    public static async Task CheckEnsureCreated(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();

        if (dataContext == null || !dataContext.CheckConnection())
        {
            Console.WriteLine("Could not connect to database...");
            // Console.WriteLine("ConnectionString" + app.Configuration.GetConnectionString("PostgreSQL"));
        }
        
        if (app.Environment.IsDevelopment())
        {
            var deleteCreate = app.Configuration.GetValue(typeof(string), "EFCORE_DELETECREATE")?.ToString();
            if (deleteCreate == "true")
            {
                Console.WriteLine("Database ensuring deleted and created...");
                await dataContext.Database.EnsureDeletedAsync();
                await dataContext.Database.EnsureCreatedAsync();

                await SeedJMDictData.Run(serviceScope);
                await SeedUserData.Run(serviceScope);
                await SeedTrackingData.Run(serviceScope);
            }
            else
            {
                Console.WriteLine("Database ensuring created...");
                await dataContext.Database.EnsureCreatedAsync();
            }
        }
    }
}