using Application.Mappings;
using Application.Response;
using Application.Services;
using Application.UseCaseCommands;
using AutoMapper;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class ReviewEntryUnitTests
{
    private readonly ITestOutputHelper _output;
    
    private readonly Mock<ITrackingRepository> _mockTrackingRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly AddEntryEvent _handler;
    private readonly Mock<ITimeService> _time;
    
    public ReviewEntryUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        
        _mockTrackingRepo = new Mock<ITrackingRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _time = new Mock<ITimeService>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _handler = new AddEntryEvent(_mockTrackingRepo.Object, _unitOfWork.Object, _time.Object);
    }
    
    [Fact]
    public async void AddReviewEntry_ValidInput_ReturnsNoContent()
    {
        // Arrange
        var request = new AddEntryEventRequest { ent_seq = "1234", UserId = new Guid(), EventType = EventType.Review, Value = "Okay" };
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedResult = Response<object>.NoContent();
        result.Should().BeEquivalentTo(expectedResult);
    }
}