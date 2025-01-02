using Application.UseCaseCommands;
using Domain.RepositoryInterfaces;
using Moq;
using Tests.ApplicationUnitTests;
using Xunit.Abstractions;
using Xunit.Sdk;
using AddEntryToTag = Application.UseCaseCommands.AddEntryToTag;

namespace Runner;

class Runner
{
    // Used for plain running things, so I can see the Console print, which doesn't show up when running tests.
    static void Main(string[] args)
    {
        Console.WriteLine("Runner start");

        ApplicationUnitTestRunners.AddEntryToTag_GetByentseqNull_ThrowsProblemException();
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
}