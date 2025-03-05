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

namespace Presentation.Controllers;

public record CreateTagRequest
{
    public string Name { get; init; }
    public Guid UserId { get; init; }
}

[ApiController]
public class TagController : BaseApiController
{
    private readonly ICrudService<Tag, TagDto> _crudService;
    private readonly IMediator _mediator;
    public TagController(ICrudService<Tag, TagDto> crudService, IMediator mediator)
    {
        _crudService = crudService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var response = await _crudService.GetDtoByIdAsync(id);
        return this.ToActionResult(response);
    }

    [HttpGet("/api/Tags")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetAllByUser(CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if(userId.Equals(Guid.Empty)) return BadRequest();
        
        var request = new TagsByUserIdRequest { UserId = userId };

        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request) // uses CrudService for convenience
    {
        if (request.Name is null) return BadRequest();

        // replace with getting userId from Identity
        var userId = new Guid(User.FindFirst("Id")!.Value);
        
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = userId
        };
        
        var response = await _crudService.CreateAsync(tag);

        return this.ToActionResult(response);
    }

    [HttpPost("add-entry")]
    public async Task<IActionResult> AddEntry([FromBody] AddEntryToTagRequest request, CancellationToken cancellationToken)
    {
        // replace with getting userId from Identity
        // var userId = new Guid(User.FindFirst("Id")!.Value);

        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
}