using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseCommands;
using AutoMapper;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using Application.Response;
using Application.UseCaseQueries;
using Microsoft.AspNetCore.Authorization;
using Tag = Domain.Entities.Tracking.Tag;

namespace Presentation.Controllers;

[ApiController]
public class TagController : BaseApiController
{
    private readonly ICrudService<Tag, Tag_EITDto> _crudService;
    private readonly IMediator _mediator;
    public TagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var response = await _crudService.GetDtoByIdAsync(id);
        return this.ToActionResult(response);
    }

    [HttpGet("/api/tags/entryIsTagged")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetAllTag_EITByUser(CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if(userId.Equals(Guid.Empty)) return BadRequest();
        
        var request = new TagsByUserIdRequest<Tag_EITDto>(userId);

        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
    
    [HttpGet("/api/tags")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetAllTagsByUser(CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if(userId.Equals(Guid.Empty)) return BadRequest();
        
        var request = new TagsByUserIdRequest<TagDto>(userId);

        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
    
    // public record CreateTagRequest
    // {
    //     public string Name { get; init; }
    //     public Guid UserId { get; init; }
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Create([FromBody] CreateTagRequest request) // uses CrudService for convenience
    // {
    //     if (request.Name is null) return BadRequest();
    //
    //     // replace with getting userId from Identity
    //     var userId = new Guid(User.FindFirst("Id")!.Value);
    //     
    //     var tag = new Tag
    //     {
    //         Id = Guid.NewGuid(),
    //         Name = request.Name,
    //         UserId = userId
    //     };
    //     
    //     var response = await _crudService.CreateAsync(tag);
    //
    //     return this.ToActionResult(response);
    // }

    // [HttpPost("{tagId}/entry/{ent_seq}")]
    // [Authorize(Roles="User")]
    // public async Task<IActionResult> TagOneEntry(Guid tagId, string ent_seq, CancellationToken cancellationToken)
    // {
    //     var userId = new Guid(User.FindFirst("Id")!.Value);
    //     if(userId.Equals(Guid.Empty)) return BadRequest();
    //
    //     var request = new AddEntryToTagRequest
    //     {
    //         ent_seq = ent_seq,
    //         TagId = tagId,
    //         UserId = userId
    //     };
    //
    //     var response = await _mediator.Send(request, cancellationToken);
    //
    //     return this.ToActionResult(response);
    // }
}