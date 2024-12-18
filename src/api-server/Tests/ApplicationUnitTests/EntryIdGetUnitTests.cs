using Application.Mappings;
using Application.Mappings.EntityDtos;
using Application.Response;
using Application.UseCaseQueries;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class EntryIdGetUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<IEntryRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly EntryIdGet _handler;
    
    public EntryIdGetUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        _mockRepo = new Mock<IEntryRepository>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _handler = new EntryIdGet(_mockRepo.Object, _mapper);
    }

    [Fact]
    public async void EntryIdGetHandle_ValidInput_ReturnsEntryDto()
    {
        // Arrange
        _mockRepo.Setup(service => service.GetBy_ent_seq(It.IsAny<string>()))
            .ReturnsAsync( new Entry { ent_seq = "1234" } );
        
        var request = new EntryIdGetRequest { ent_seq = "1234" }; 
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedResult = Response<EntryDto>.Ok("Entry found", new EntryDto { ent_seq = "1234" });
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async void EntryIdGetHandle_DbThrows_HandleThrows()
    {
        // Arrange
        _mockRepo.Setup(service => 
            service.GetBy_ent_seq(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));
        
        var request = new EntryIdGetRequest { ent_seq = "1234" }; 
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => 
            await _handler.Handle(request, It.IsAny<CancellationToken>()));
    }
}