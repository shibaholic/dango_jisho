using Application;
using Application.Mappings;
using Application.Mappings.EntityDtos;
using Application.Response;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using AutoMapper;
using Domain.Entities.JMDict;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class AddEntryToTagUnitTests
{
    private readonly ITestOutputHelper _output;

    private readonly Mock<ITagRepository> _mockTagRepo;
    private readonly Mock<IEntryRepository> _mockEntryRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly AddEntryToTag _handler;
    
    public AddEntryToTagUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        
        _mockTagRepo = new Mock<ITagRepository>();
        _mockEntryRepo = new Mock<IEntryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _handler = new AddEntryToTag(_mockTagRepo.Object, _mockEntryRepo.Object, _unitOfWork.Object, _mapper);
    }
    
    [Fact]
    public async void AddEntryToTag_ValidInput_ReturnsNoContent()
    {
        // Arrange
        _mockEntryRepo.Setup(service =>
                service.GetBy_ent_seq(It.IsAny<string>()))
            .ReturnsAsync(new Entry());
        
        _mockTagRepo.Setup(service =>
                service.CreateEntryIsTaggedAsync(It.IsAny<EntryIsTagged>()))
            .ReturnsAsync(new EntryIsTagged());

        _mockTagRepo.Setup(service =>
                service.ReadByIdUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Tag());
        
        var request = new AddEntryToTagRequest { ent_seq = "1234", TagId = new Guid(), UserId = new Guid() };
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedResult = Response<object>.NoContent("Entry added to tag");
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async void AddEntryToTag_GetByentseqNull_ThrowsProblemException()
    {
        // Arrange
        _mockEntryRepo.Setup(service =>
                service.GetBy_ent_seq(It.IsAny<string>()))
            .ReturnsAsync((Entry?)null);
        
        var request = new AddEntryToTagRequest { ent_seq = "1234", TagId = new Guid(), UserId = new Guid() };
        
        // Act & Assert
        await _handler.Invoking(h => h.Handle(request, new CancellationToken()))
            .Should().ThrowAsync<ProblemException>()
            .WithMessage($"Entry with ent_seq: {request.ent_seq} does not exist.");
    }
}