using Application.Mappings.EntityDtos;
using Application.MediatrPipeline;
using Application.Response;
using Application.UseCaseQueries;
using Domain.Entities;
using MediatR;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class MediatrPipelineUnitTests
{
    private readonly ITestOutputHelper _output;

    public MediatrPipelineUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Mediatr ExceptionHandlingPipelineBehavior unit test to test the conversion of thrown exceptions to Response.Failure
    /// </summary>
    [Fact]
    public async Task ExceptionPipeline_EntryIdQuery_ThrowsException()
    {
        // Arrange
        var exceptionPipeline = new ExceptionHandlingPipelineBehavior<EntryIdGetRequest, Response<EntryDto>>();
        
        var mockHandler = new Mock<IRequestHandler<EntryIdGetRequest, Response<EntryDto>>>();
        mockHandler.Setup(h => h.Handle(It.IsAny<EntryIdGetRequest>(),
                It.IsAny<CancellationToken>()))
            .Throws(new Exception("Test exception"));
        
        var request = new EntryIdGetRequest();
        
        // Act
        var result = await exceptionPipeline.Handle(request,
                async () => await mockHandler.Object.Handle(request, CancellationToken.None),
                CancellationToken.None);
        
        // Assert
        Assert.Equal(Status.ServerError, result.Status);
        Assert.False(result.Successful);
    }
    
    [Fact]
    public async Task ExceptionPipeline_EntryIdQuery_ReturnsResponse()
    {
        // Arrange
        var response = Response<EntryDto>.Ok("Test result", new EntryDto { ent_seq = "1234" });
        
        var exceptionPipeline = new ExceptionHandlingPipelineBehavior<EntryIdGetRequest, Response<EntryDto>>();
        
        var mockHandler = new Mock<IRequestHandler<EntryIdGetRequest, Response<EntryDto>>>();
        mockHandler.Setup(h => h.Handle(It.IsAny<EntryIdGetRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        var request = new EntryIdGetRequest();
        
        // Act
        var result = await exceptionPipeline.Handle(request,
            async () => await mockHandler.Object.Handle(request, CancellationToken.None),
            CancellationToken.None);
        
        // Assert
        Assert.Equal(response, result);
    }
}