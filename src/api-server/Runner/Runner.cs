using Application.UseCaseCommands;
using Domain.RepositoryInterfaces;
using Moq;
using Tests.ApplicationUnitTests;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Runner;

class Runner
{
    static void Main(string[] args)
    {
        Console.WriteLine("Runner start");

        ApplicationUnitTestRunners.ImportJMdict_ValidInput_ReturnsEntry();
    }
}

file class ApplicationUnitTestRunners
{
    public static void ImportJMdict_ValidInput_ReturnsEntry()
    {
        var _output = new TestOutputHelper();
        var Tests = new ImportJMdictUnitTests(_output);
        
        Tests.ImportJMdict_ValidInput_ReturnsEntry();
    }
}