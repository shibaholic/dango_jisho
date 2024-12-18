using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Presentation;
using Presentation.Controllers;
using Xunit.Abstractions;

namespace Tests.PresentationUnitTests;

public class WeatherForecastUnitTests
{
    private readonly ITestOutputHelper _output;

    public WeatherForecastUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void Get_NoInput_ReturnsNotEmpty()
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