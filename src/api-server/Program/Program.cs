using Application;
using Infrastructure;
using Infrastructure.DbContext;
using Presentation;

namespace Program;

public static class Program
{
    public static void Main(string[] args)
    {
        // BUILD
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.ConfigurePresentationServices();
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices(builder.Configuration);
        
        // APP
        var app = builder.Build();
    
        app.RunPresentationServices();
        app.CheckIfDatabaseCreated();
        
        app.Run();
    }
    
    private static void CheckIfDatabaseCreated(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
    
        if (dataContext!.CheckConnection())
        {
            if (app.Environment.IsDevelopment())
            {
                // dataContext.Database.EnsureDeleted();
                dataContext.Database.EnsureCreated();
            }
        }
        else
        {
            Console.Error.WriteLine("Could not connect to database.");
        }
    }
}