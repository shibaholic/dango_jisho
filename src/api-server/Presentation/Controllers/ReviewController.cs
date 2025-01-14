using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseCommands;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class ReviewController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICrudService<EntryEvent, ReviewEventDto> _crudService;
    
    public ReviewController(IMediator mediator, ICrudService<EntryEvent, ReviewEventDto> crudService)
    {
        _mediator = mediator;
        _crudService = crudService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddEntryEventRequest eventRequest, CancellationToken cancellationToken)
    {
        // replace with getting userId from Identity
        // var userId = new Guid(User.FindFirst("Id")!.Value);
        
        var response = await _mediator.Send(eventRequest, cancellationToken);
        
        return this.ToActionResult(response);
    }
}