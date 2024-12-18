using System.Text;
using Application.Automapper.EntityDtos;
using Application.Response;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Xunit.Abstractions;

namespace Tests.PresentationUnitTests;

public class EntryControllerUnitTests {
    private readonly ITestOutputHelper _output;

    private readonly Mock<IMediator> _mockMediator;
    private readonly EntryController _controller;
    public EntryControllerUnitTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Arrange
        _mockMediator = new Mock<IMediator>();
        _controller = new EntryController(_mockMediator.Object);
    }
    
    [Fact]
    public async void GetById_ValidInput_ReturnsOk()
    {
        // Arrange
        var response = Response<EntryDto>.Ok("Entry found", new EntryDto { ent_seq = "1234" });
        
        _mockMediator.Setup(m => m.Send(It.IsAny<EntryIdGetRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        var request = new EntryIdGetRequest {ent_seq = "1234"};
        
        // Act
        var result = await _controller.GetById(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response.Data, (okResult.Value as Response<EntryDto>).Data);
    }
    
    [Fact]
    public async void GetById_MediatrUnsuccessful_ReturnsBadRequest()
    {
        // Arrange
        var response = Response<EntryDto>.ServerError("Test error");
        
        _mockMediator.Setup(m => m.Send(It.IsAny<EntryIdGetRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        EntryIdGetRequest? request = new EntryIdGetRequest {ent_seq = "1234"};
        
        // Act
        var result = await _controller.GetById(request);
        
        // Assert
        
        // To test for HttpStatusCode 500 ServerError use ObjectResult.
        var serverErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(response.Message, serverErrorResult.Value);
    }

    [Fact]
    public async void UploadJMdict_ValidInput_ReturnsNoContent()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var fileContent = "<root><message>Hello, XML!</message></root>";
        var fileName = "test.xml";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.ContentType).Returns("application/xml");
        
        var formFile = mockFile.Object;
        
        var payload = new EntryController.UploadJMdictPayload(formFile);
        
        _mockMediator.Setup(m => m.Send(It.IsAny<ImportJMdictRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response<object>.NoContent("Test success"));
        
        // Act
        var result = await _controller.UploadJMdict(payload, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async void UploadJMdict_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var fileContent = "<root><message>Hello, XML!</message></root>";
        var fileName = "test.xml";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.ContentType).Returns("application/xml");
        
        var formFile = mockFile.Object;
        
        var payload = new EntryController.UploadJMdictPayload(formFile);
        
        _mockMediator.Setup(m => m.Send(It.IsAny<ImportJMdictRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response<object>.BadRequest("Test fail"));
        
        // Act
        var result = await _controller.UploadJMdict(payload, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}