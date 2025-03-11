using Application.Mappings;
using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.JMDict;
using Application.Response;
using Application.UseCaseQueries;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.RepositoryInterfaces;
using FluentAssertions;
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
    public async Task Search_ValidInput_ReturnsEntryDto()
    {
        // Arrange
        List<Entry> repoResponse = new List<Entry> { new Entry { ent_seq = "1234" } };
        _mockRepo.Setup(repo => repo.Search(It.IsAny<string>()))
            .ReturnsAsync(repoResponse);

        var request = new EntryQueryRequest();
        
        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Status.Should().Be(Status.Ok);
        var expectedResponseData = new List<Entry_TEDto> { new Entry_TEDto() { ent_seq = "1234" } };
        response.Data.Should().BeEquivalentTo(expectedResponseData);
    }
}