using Application.Automapper.EntityDtos;
using Application.Mappings;
using Application.Response;
using Application.UseCaseQueries;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Infrastructure.Repositories;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class EntryQueryUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<IEntryRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly EntryQuery _handler;
    
    public EntryQueryUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        _mockRepo = new Mock<IEntryRepository>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _handler = new EntryQuery(_mockRepo.Object, _mapper);
    }

    [Fact]
    public async void EntryQueryHandle_ValidInput_ReturnsEntry()
    {
        // Arrange
        _mockRepo.Setup(service => service.GetBy_ent_seq(It.IsAny<string>()))
            .ReturnsAsync( new Entry { ent_seq = "1234" } );
        
        var request = new EntryQueryRequest { ent_seq = "1234" }; 
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedResult = Response<EntryDto>.Ok("Entry found", new EntryDto { ent_seq = "1234" });
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async void EntryQueryHandle_DbThrows_ReturnsFailure()
    {
        // Arrange
        _mockRepo.Setup(service => 
            service.GetBy_ent_seq(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));
        
        var request = new EntryQueryRequest { ent_seq = "1234" }; 
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => 
            await _handler.Handle(request, It.IsAny<CancellationToken>()));
    }
}