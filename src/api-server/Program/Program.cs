using Application;
using Infrastructure;
using Infrastructure.DbContext;
using Presentation;

namespace Program;

public class Program
{
    public static void Main(string[] args)
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
            if (app.Environment.IsDevelopment())
            {
                var deleteCreate = (string)app.Configuration.GetValue(typeof(string), "EFCORE_DELETECREATE");
                if (deleteCreate == "true")
                {
                    dataContext.Database.EnsureDeleted();
                    dataContext.Database.EnsureCreated();
                    Console.WriteLine("Database ensure deleted and created.");
                }
                else
                {
                    dataContext.Database.EnsureCreated();
                    Console.WriteLine("Database ensure created.");
                }
                
            }
        }
        else
        {
            Console.Error.WriteLine("Could not connect to database.");
        }
    }
}