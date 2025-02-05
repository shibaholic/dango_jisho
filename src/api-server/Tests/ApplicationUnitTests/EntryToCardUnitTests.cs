using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Application.Utilities;
using Domain.Entities.CardData;
using Domain.Entities.JMDict;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class EntryToCardInputOutput
{
  public Entry InputEntry { get; init; }
  public Card OutputCard { get; init; }

  public EntryToCardInputOutput() {}

  public EntryToCardInputOutput(Entry input, Card output)
  {
      InputEntry = input;
      OutputCard = output;
  }
}

public class EntryToCardUnitTests
{
    private readonly ITestOutputHelper _output;
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { 
        PropertyNameCaseInsensitive = true, 
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public EntryToCardUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> GetTestCases()
    {
        string testsDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName,
            "Tests");
        string testDataDir = "TestData";
        string testFileName = "EntryToCardTestCases.json";
        string filePath = Path.Combine(testsDir, testDataDir, testFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Could not find test case" + filePath, filePath);
        }
        
        var testCaseJson = File.ReadAllText(filePath);
        
        var testCases = JsonSerializer.Deserialize<List<EntryToCardInputOutput>>(testCaseJson, JsonOptions);

        if (testCases == null) throw new SerializationException("Could not deserialize.");

        return testCases.Select(tc => new object[] { tc });
    }

    [Theory]
    [MemberData(nameof(GetTestCases), MemberType = typeof(EntryToCardUnitTests))]
    public void EntryToCard_ComplexInput_ComplexOutput(EntryToCardInputOutput inputOutput)
    {
      // Arrange
      
      // Act
      var card = EntryToCard.Convert(inputOutput.InputEntry);
      
      // _output.WriteLine($"entry.KanjiElement.Ids: ");
      // inputOutput.InputEntry.KanjiElements.Select(ke => ke.Id).ToList().ForEach(kanjiId => _output.WriteLine($"{kanjiId}"));
      // _output.WriteLine($"card.KanjiId: {card.KanjiId}");
      // _output.WriteLine($"expected card.KanjiId: {inputOutput.OutputCard.KanjiId}");
      
      // Assert
      card.Should().BeEquivalentTo(inputOutput.OutputCard);
    }
}