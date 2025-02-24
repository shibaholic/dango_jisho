using Application;
using Application.Response;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class EntryController : BaseApiController
{
    private readonly IMediator _mediatr;
    public EntryController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByEnt_Seq([FromQuery] string ent_seq)
    {
        var request = new EntryEntSeqGetRequest { ent_seq = ent_seq };
        
        var response = await _mediatr.Send(request);
        
        return this.ToActionResult(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var request = new EntryQueryRequest { query = query };
        
        var response = await _mediatr.Send(request);
        
        return this.ToActionResult(response);
    }

    public record UploadJMdictPayload(
        IFormFile File
    );

    [HttpPost]
    [Authorize(Roles = "Admin")]
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
        
        return this.ToActionResult(response);
    }
}