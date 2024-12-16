using Application.Response;
using Application.UseCaseQueries;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Repositories;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class ApplicationQueryUnitTests
{
    private readonly ITestOutputHelper _output;

    public ApplicationQueryUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void EntryQueryHandle_ValidInput_ReturnsEntry()
    {
        // Arrange
        var mockRepo = new Mock<IEntryRepository>();
        mockRepo.Setup(service => service.GetBy_ent_seq(It.IsAny<string>()))
            .ReturnsAsync( new Entry { ent_seq = "1234" } );
        
        var entryQueryUseCase = new EntryQuery(mockRepo.Object);
        
        var request = new EntryQueryRequest { ent_seq = "1234" }; 
        
        // Act
        var result = await entryQueryUseCase.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data.ent_seq);
    }
    
    [Fact]
    public async void EntryQueryHandle_DbThrows_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IEntryRepository>();
        mockRepo.Setup(service => service.GetBy_ent_seq(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));
        
        var entryQueryUseCase = new EntryQuery(mockRepo.Object);
        
        var request = new EntryQueryRequest { ent_seq = "1234" }; 
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await entryQueryUseCase.Handle(request, It.IsAny<CancellationToken>()));
    }
}