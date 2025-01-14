using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseCommands;
using AutoMapper;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using Application.Response;

namespace Presentation.Controllers;

[ApiController]
[Route("/tag")]
public class TagController : ControllerBase
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
        Tag? result = await _crudService.GetByIdAsync(id);
        
        if (result == null) return NotFound();
        
        return Ok(Response<Tag>.Ok("Found", result));
    }

    public record CreateTagRequest
    {
        public string Name { get; init; }
        public Guid UserId { get; init; }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request) // uses CrudService for convenience
    {
        if (request.Name is null) return BadRequest();

        // replace with getting userId from Identity
        // var userId = new Guid(User.FindFirst("Id")!.Value);
        
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = request.UserId
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