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
    private readonly Mock<ITrackingRepository> _mockTrackingRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly AddEntryToTag _handler;
    
    public AddEntryToTagUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        
        _mockTagRepo = new Mock<ITagRepository>();
        _mockEntryRepo = new Mock<IEntryRepository>();
        _mockTrackingRepo = new Mock<ITrackingRepository>(); // not used. maybe will actually use if testing db exceptions
        _unitOfWork = new Mock<IUnitOfWork>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _handler = new AddEntryToTag(_mockTagRepo.Object, _mockEntryRepo.Object, _mockTrackingRepo.Object, _unitOfWork.Object, _mapper);
    }
    
    [Fact]
    public async void AddEntryToTag_ValidInputNotNullTracking_ReturnsNoContentNoSideEffect()
    {
        // Arrange
        _mockEntryRepo.Setup(service =>
                service.ReadByEntSeq(It.IsAny<string>()))
            .ReturnsAsync(new Entry {});

        _mockTagRepo.Setup(service =>
                service.ReadByIdUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Tag {});
        
        _mockTrackingRepo.Setup(service =>
            service.ReadTrackedEntryByIdsAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(new TrackedEntry {});
        
        var request = new AddEntryToTagRequest { ent_seq = "1234", TagId = new Guid(), UserId = new Guid() };
        
        // Act
        var result = await _handler.Handle(request, new CancellationToken());
        
        // Assert
        var expectedResult = Response<object>.NoContent();
        result.Should().BeEquivalentTo(expectedResult);
        
        _mockTrackingRepo.Verify(service => 
            service.CreateTrackedEntryAsync(It.IsAny<TrackedEntry>()), 
            Times.Never
        );
    }
    
    [Fact]
    public async void AddEntryToTag_ValidInputNullTracking_ReturnsNoContentWithSideEffect()
    {
        // Arrange
        _mockEntryRepo.Setup(service =>
                service.ReadByEntSeq(It.IsAny<string>()))
            .ReturnsAsync(new Entry());

        _mockTagRepo.Setup(service =>
                service.ReadByIdUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Tag());
        
        _mockTrackingRepo.Setup(service =>
                service.ReadTrackedEntryByIdsAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((TrackedEntry?)null);
        
        var request = new AddEntryToTagRequest { ent_seq = "1234", TagId = new Guid(), UserId = new Guid() };
        
        // Act
        var result = await _handler.Handle(request, new CancellationToken());
        
        // Assert
        var expectedResult = Response<object>.NoContent();
        result.Should().BeEquivalentTo(expectedResult);
        
        _mockTrackingRepo.Verify(service => 
                service.CreateTrackedEntryAsync(It.IsAny<TrackedEntry>()), 
            Times.Once
        );
    }
    
    [Fact]
    public async void AddEntryToTag_GetByentseqNull_ThrowsProblemException()
    {
        // Arrange
        _mockEntryRepo.Setup(service =>
                service.ReadByEntSeq(It.IsAny<string>()))
            .ReturnsAsync((Entry?)null);
        
        var request = new AddEntryToTagRequest { ent_seq = "1234", TagId = new Guid(), UserId = new Guid() };
        
        // Act & Assert
        await _handler.Invoking(h => h.Handle(request, new CancellationToken()))
            .Should().ThrowAsync<ProblemException>()
            .WithMessage($"Entry with ent_seq: {request.ent_seq} does not exist.");
    }
}