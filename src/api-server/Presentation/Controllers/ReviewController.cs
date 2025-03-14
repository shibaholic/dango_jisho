using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseCommands;
using Application.UseCaseQueries;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class ReviewController : BaseApiController
{
    private readonly IMediator _mediator;
    
    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record ReviewEventRequest
    {
        public string ent_seq { get; set; }
        public string Value { get; set; }
        public EventType EventType { get; init; }
    }
    
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ReviewEvent([FromBody] ReviewEventRequest request, CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if (userId.Equals(Guid.Empty)) return BadRequest();

        var handlerRequest = new AddEntryEventRequest
        {
            UserId = userId,
            ent_seq = request.ent_seq,
            Value = request.Value,
            EventType = request.EventType
        };
        
        var response = await _mediator.Send(handlerRequest, cancellationToken);
        
        return this.ToActionResult(response);
    }

    [HttpGet("tag/{tagId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> NextReview(Guid tagId, CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if (userId.Equals(Guid.Empty)) return BadRequest();

        var request = new NextReviewRequest { TagId = tagId, UserId = userId };

        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
}