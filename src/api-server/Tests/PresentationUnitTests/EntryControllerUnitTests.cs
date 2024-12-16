using Application.Automapper.EntityDtos;
using Application.Response;
using Application.UseCaseQueries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Xunit.Abstractions;

namespace Tests.PresentationUnitTests;

public class EntryControllerUnitTests {
    private readonly ITestOutputHelper _output;

    public EntryControllerUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void GetById_ValidInput_ReturnsOkResultObject()
    {
        // Arrange
        var response = Response<EntryDto>.Ok("Entry found", new EntryDto { ent_seq = "1234" });
        
        var mockMediatr = new Mock<IMediator>();
        mockMediatr.Setup(m => m.Send(It.IsAny<EntryQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new EntryController(mockMediatr.Object);

        var request = new EntryQueryRequest {ent_seq = "1234"};
        
        // Act
        var result = await controller.GetById(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response.Data, okResult.Value);
    }
    
    [Fact]
    public async void GetById_MediatrUnsuccessful_ReturnsBadRequest()
    {
        // Arrange
        var response = Response<EntryDto>.ServerError("Test error");
        
        var mockMediatr = new Mock<IMediator>();
        mockMediatr.Setup(m => m.Send(It.IsAny<EntryQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        var controller = new EntryController(mockMediatr.Object);
        
        EntryQueryRequest? request = new EntryQueryRequest {ent_seq = "1234"};
        
        // Act
        var result = await controller.GetById(request);
        
        // Assert
        
        // To test for HttpStatusCode 500 ServerError use ObjectResult.
        var serverErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(response.Message, serverErrorResult.Value);
    }
}