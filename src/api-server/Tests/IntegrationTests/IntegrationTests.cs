using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Xunit.Abstractions;

namespace Tests.IntegrationTests;

using WebAppFactory = ChuuiWebAppFactory<Program.Program>;

public class IntegrationTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;
    private readonly WebAppFactory _factory;
    private readonly ITestOutputHelper _output;

    public IntegrationTests(WebAppFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task UploadJMdict_Post_ReturnsOk()
    {
        // Arrange
        string testsDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName,
            "Tests");
        string testDataDir = "TestData";
        string testFileName = "JMdict_10k.xml";
        string filePath = Path.Combine(testsDir, testDataDir, testFileName);
        if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));

        var formFile = new FormFile(stream, 0, stream.Length, "file", testFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/xml"
        };
        
        // Create the content for a Multipart form-data request
        using var content = new MultipartFormDataContent();
        var fileContentStream = new StreamContent(formFile.OpenReadStream())
        {
            Headers =
            {
                ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType)
            }
        };
        content.Add(fileContentStream, nameof(EntryController.UploadJMdictPayload.File), formFile.FileName);

        // Act
        var response = await _client.PostAsync("/api/entry", content);

        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
    }
    
    [Fact]
    public async Task WeatherForecast_Get_ReturnsOk()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/WeatherForecast");

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);
    }
}