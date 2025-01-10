using Application.Mappings;
using Application.Mappings.EntityDtos.Tracking;
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

public class StartStudySetUnitTests
{
    private readonly ITestOutputHelper _output;
    
    private readonly Mock<IStudySetRepository> _ssRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly StartStudySet _handler;
    private readonly Mock<ITimeService> _timeService;
    
    public StartStudySetUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        _ssRepo = new Mock<IStudySetRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Mappings>()
        );
        _mapper = mapperConfig.CreateMapper();
        
        _timeService = new Mock<ITimeService>();
        
        _handler = new StartStudySet(_ssRepo.Object, _unitOfWork.Object, _mapper, _timeService.Object);
    }
    
    [Fact]
    public async void StartStudySet_ValidInput_ReturnsOk()
    {
        // Arrange
        var ssRepoResponse = new StudySet
            { Id = new Guid(), UserId = new Guid(), LastStartDate = null, NewEntryGoal = 30, NewEntryCount = 0 };
        _ssRepo.Setup(repo => repo.ReadByIdUserId(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(ssRepoResponse);
        
        var request = new StartStudySetRequest { StudySetId = new Guid(), UserId = new Guid() };
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedStudySetDto = new StudySetDto { Id = new Guid(), UserId = new Guid(), LastStartDate = null, NewEntryGoal = 30, NewEntryCount = 0 };
        var expectedResult = Response<StudySetDto>.Ok("Study set started.", expectedStudySetDto);
        result.Should().BeEquivalentTo(expectedResult);
    }

    public static string StartMessage = "Study set started.";
    public static string RestartMessage = "Study set not re-started.";
    public static IEnumerable<object[]> LastStartDateData =>
        new List<object[]>
        {
            new object[] { new DateTime(2025, 1, 1, 23, 0, 0), StartMessage },
            new object[] { new DateTime(2025, 1, 2, 0, 0, 0), RestartMessage },
            new object[] { new DateTime(2025, 1, 2, 0, 0, 0), RestartMessage },
            new object[] { new DateTime(2025, 1, 2, 0, 0, 0), StartMessage }
        };
    
    [Theory, MemberData(nameof(LastStartDateData))]
    public async void StartStudySet_LastStartDates_ReturnsOk(DateTime lastStartDate, string responseMessage)
    {
        // Arrange
        var ssRepoResponse = new StudySet
            { Id = new Guid(), UserId = new Guid(), LastStartDate = lastStartDate, NewEntryGoal = 30, NewEntryCount = 0  }; 
        // DateTime.Now.ToOffset(TimeSpan.FromHours(1)).AddHours(-12), NewEntryGoal = 30, NewEntryCount = 0
        _ssRepo.Setup(repo => repo.ReadByIdUserId(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(ssRepoResponse);

        _timeService.Setup(service => service.Now)
            .Returns(new DateTime(2025, 1, 2, 0, 0, 0));
        
        var request = new StartStudySetRequest { StudySetId = new Guid(), UserId = new Guid() };
        
        // Act
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        
        // Assert
        var expectedStudySetDto = new StudySetDto { Id = new Guid(), UserId = new Guid(), LastStartDate = ssRepoResponse.LastStartDate, NewEntryGoal = 30, NewEntryCount = 0  };
        var expectedResult = Response<StudySetDto>.Ok(responseMessage, expectedStudySetDto);
        
        result.Should().BeEquivalentTo(expectedResult);
    }
}