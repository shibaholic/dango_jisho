using Presentation;

namespace Program;

public static class Program
{
    public static void Main(string[] args)
    {
        // BUILD
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.ConfigurePresentationServices();

        // APP
        var app = builder.Build();
    
        app.RunPresentationServices();

        app.Run();
    }
}