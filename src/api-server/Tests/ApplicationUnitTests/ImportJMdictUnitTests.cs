using System.Diagnostics;
using Application.Automapper.EntityDtos;
using Application.Mappings;
using Application.Response;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Infrastructure.Repositories;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class ImportJMdictUnitTests
{
    private readonly Mock<IEntryRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly ImportJMdict _handler;
    
    public ImportJMdictUnitTests(ITestOutputHelper output)
    {
        // Arrange
        _mockRepo = new Mock<IEntryRepository>();
        _mockUow = new Mock<IUnitOfWork>();
        _handler = new ImportJMdict(_mockRepo.Object, _mockUow.Object);
    }

    [Fact]
    public async void ImportJMdict_InvalidInput_ThrowsException()
    {
        // Arrange
        var fileContent = "<root><message>Hello, XML!</message></root>";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var request = new ImportJMdictRequest { Content = stream.ToArray() };
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(request, CancellationToken.None));
    }
    
    [Fact]
    public async void ImportJMdict_ValidInput_ReturnsNoContent()
    {
        // Arrange
        string testsDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName,
            "Tests");
        string testDataDir = "TestData";
        string testFileName = "JMdict_1k.xml";
        string filePath = Path.Combine(testsDir, testDataDir, testFileName);
        if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
        
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));
        var request = new ImportJMdictRequest { Content = stream.ToArray() };
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.Equal(Status.NoContent, result.Status);
        Console.WriteLine($"{result.Message}");
    }
}