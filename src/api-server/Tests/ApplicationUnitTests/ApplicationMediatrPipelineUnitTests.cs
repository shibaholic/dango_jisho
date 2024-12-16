using Application.MediatrPipeline;
using Application.Response;
using Application.UseCaseQueries;
using Domain.Entities;
using MediatR;
using Moq;
using Xunit.Abstractions;

namespace Tests.ApplicationUnitTests;

public class ApplicationMediatrPipelineUnitTests
{
    private readonly ITestOutputHelper _output;

    public ApplicationMediatrPipelineUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Mediatr ExceptionHandlingPipelineBehavior unit test to test the conversion of thrown exceptions to Response.Failure
    /// </summary>
    [Fact]
    public async Task ExceptionPipeline_EntryQuery_ThrowsException()
    {
        // Arrange
        var exceptionPipeline = new ExceptionHandlingPipelineBehavior<EntryQueryRequest, Response<Entry>>();
        
        var mockHandler = new Mock<IRequestHandler<EntryQueryRequest, Response<Entry>>>();
        mockHandler.Setup(h => h.Handle(It.IsAny<EntryQueryRequest>(),
                It.IsAny<CancellationToken>()))
            .Throws(new Exception("Test exception"));
        
        var request = new EntryQueryRequest();
        
        // Act
        var result = await exceptionPipeline.Handle(request,
                async () => await mockHandler.Object.Handle(request, CancellationToken.None),
                CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(Status.ServerError, result.Status);
        Assert.False(result.Successful);
    }
    
    [Fact]
    public async Task ExceptionPipeline_EntryQuery_ReturnsResponse()
    {
        // Arrange
        var exceptionPipeline = new ExceptionHandlingPipelineBehavior<EntryQueryRequest, Response<Entry>>();
        
        var mockHandler = new Mock<IRequestHandler<EntryQueryRequest, Response<Entry>>>();
        mockHandler.Setup(h => h.Handle(It.IsAny<EntryQueryRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Entry>("Test result", Status.Success, true));
        
        var request = new EntryQueryRequest();
        
        // Act
        var result = await exceptionPipeline.Handle(request,
            async () => await mockHandler.Object.Handle(request, CancellationToken.None),
            CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(Status.Success, result.Status);
        Assert.True(result.Successful);
    }
}