
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tests.ApplicationUnitTests;
using Xunit.Sdk;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Runner;

class Runner
{
    // Used for plain running things, so I can see the Console print, which doesn't show up when running tests.
    static async Task Main(string[] args)
    {
        Console.WriteLine("Runner start");

        // ApplicationUnitTestRunners.EntryToCard_ComplexInput_ComplexOutput();
        await InfrastructureRunners.EntryRepository_BulkReadAllAsync(args);
    }
}

file class ApplicationUnitTestRunners
{
    public static void ImportJMdict_ValidInput_ReturnsOk()
    {
        var _output = new TestOutputHelper();
        var Tests = new ImportJMdictUnitTests(_output);
        
        Tests.ImportJMdict_ValidInput_ReturnsNoContent();
    }

    public static void AddEntryToTag_GetByentseqNull_ThrowsProblemException()
    {
        var _output = new TestOutputHelper();
        var Tests = new AddEntryToTagUnitTests(_output);
        
        Tests.AddEntryToTag_GetByentseqNull_ThrowsProblemException();
    }

    public static void StartStudySet_LastStartDateSame_ReturnsOkNotRestarted()
    {
        var _output = new TestOutputHelper();
        var Tests = new StartStudySetUnitTests(_output);

        var testData = StartStudySetUnitTests.LastStartDateData;

        foreach (var testCase in testData)
        {
            Tests.StartStudySet_LastStartDates_ReturnsOk((DateTime)testCase[0], (string)testCase[1]);
        }
    }

    public static void EntryToCard_ComplexInput_ComplexOutput()
    {
        var _output = new TestOutputHelper();
        var tests = new EntryToCardUnitTests(_output);
        
        var testData = EntryToCardUnitTests.GetTestCases().ToList();
        
        // foreach (var testCase in testData)
        // {
        //     tests.EntryToCard_ComplexInput_ComplexOutput((EntryToCardInputOutput)testCase[0]);
        // }
        tests.EntryToCard_ComplexInput_ComplexOutput((EntryToCardInputOutput)testData[1][0]);
    }
}

file class InfrastructureRunners
{
    public static async Task EntryRepository_BulkReadAllAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine(Directory.GetCurrentDirectory());
        
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory()) // Ensures it looks in Runner's directory
            .AddJsonFile("appsettings.Runner.json", optional: false, reloadOnChange: true);
        
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        
        Console.WriteLine($"connectionString: {connectionString}");
        if (connectionString is null or "")
        {
            Console.WriteLine("No connection string");
            System.Environment.Exit(1);
        }
        
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        var dbContext = new MyDbContext(optionsBuilder.Options);
        
        var repo = new EntryRepository(dbContext);

        var entries = await repo.BulkReadAllAsync();

        var findEntry = entries.Where(e => e.ent_seq == "1014020").FirstOrDefault();
        
        Console.WriteLine(findEntry);

        Console.WriteLine();

        var findEntryJson = JsonSerializer.Serialize(findEntry);
        
        Console.WriteLine(findEntryJson);

        // Console.WriteLine(entries[1]);
    }
}