using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Presentation;
using Presentation.Controllers;
using Xunit.Abstractions;

namespace Tests.PresentationUnitTests;

public class PresentationUnitTests
{
    private readonly ITestOutputHelper _output;

    public PresentationUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void Get_NoInput_ReturnsOkResult()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<WeatherForecastController>>();
        var controller = new WeatherForecastController(mockLogger.Object);
        
        // Act
        var result = controller.Get();
        
        // Assert
        Assert.NotEmpty(result);
    }
}