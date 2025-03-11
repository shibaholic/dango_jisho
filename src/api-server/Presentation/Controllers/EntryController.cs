using System.Threading.Channels;
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
    
    [HttpGet("{ent_seq}")]
    public async Task<IActionResult> GetByEnt_Seq(string ent_seq)
    {
        Guid? userId = null;
        if (Guid.TryParse(User.FindFirst("Id")?.Value, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        
        var request = new EntryEntSeqGetRequest { ent_seq = ent_seq, UserId = userId.GetValueOrDefault()};
        
        var response = await _mediatr.Send(request);
        
        return this.ToActionResult(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        Guid? userId = null;
        if (Guid.TryParse(User.FindFirst("Id")?.Value, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var request = new EntryQueryRequest { query = query, UserId = userId };
        
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

    public class BatchTagEntryEndpointRequest
    {
        public Dictionary<Guid, bool> TagValues { get; set; } = null!;
    }
    
    [HttpPost("{ent_seq}/tags")]
    [Authorize(Roles="User")]
    public async Task<IActionResult> BatchTagEntry(string ent_seq, [FromBody] BatchTagEntryEndpointRequest request, CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if(userId.Equals(Guid.Empty)) return BadRequest();

        var handlerRequest = new TagBatchEntriesRequest
        {
            ent_seq = ent_seq,
            TagValues = request.TagValues,
            UserId = userId
        };

        var response = await _mediatr.Send(handlerRequest, cancellationToken);

        return this.ToActionResult(response);
    }
}