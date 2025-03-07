using System.Diagnostics;
using Application.UseCaseCommands;
using MediatR;

namespace Program.SeedService;

public static class SeedJMDictData
{
    public static async Task Run(IServiceScope serviceScope)
    {
        Console.WriteLine("Seeding with JMDict data...");
        
        var mediator = serviceScope.ServiceProvider.GetService<IMediator>();
        
        if (mediator == null)
        {
            Console.WriteLine("  Error while getting Mediator");
            System.Environment.Exit(1);
        }
        
        string filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "JMdict_e.xml");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("  Could not find seed data.");
            System.Environment.Exit(1);
        }
        
        var stopwatch = Stopwatch.StartNew();
        
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));
        var request = new ImportJMdictRequest { Content = stream.ToArray() }; // , Count = 1000 
        
        stopwatch.Stop();
        Console.WriteLine($"  Loading JMdict from {filePath} took {stopwatch.ElapsedMilliseconds} ms.");
        stopwatch.Restart();
        
        var result = await mediator.Send(request, CancellationToken.None);
                
        if (!result.Successful)
        {
            Console.WriteLine("  Error while importing JMdict_e.xml.");
            System.Environment.Exit(1);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"JMDict seeding took {stopwatch.ElapsedMilliseconds} ms.\n");
    }
}