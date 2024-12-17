using Application.Response;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class EntryController : ControllerBase
{
    private readonly IMediator _mediatr;

    public EntryController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] EntryQueryRequest request)
    {
        var response = await _mediatr.Send(request);
        
        // ToDo: Also need to implement Automapper and Dtos
        return ResultMapper.ToActionResult(response);
    }

    public record UploadJMdictPayload(
        IFormFile File
    );

    [HttpPost]
    public async Task<IActionResult> UploadJMdict([FromForm] UploadJMdictPayload payload, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await payload.File.CopyToAsync(memoryStream);

        var fileBytes = memoryStream.ToArray();
        var command = new ImportJMdictRequest()
        {
            Content = fileBytes
        };
        
        var response = await _mediatr.Send(command, cancellationToken);
        
        return ResultMapper.ToActionResult(response);
    }
}   