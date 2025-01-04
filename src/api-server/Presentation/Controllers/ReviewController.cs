using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseCommands;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("/entry/review")]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICrudService<ReviewEvent, ReviewEventDto> _crudService;
    
    public ReviewController(IMediator mediator, ICrudService<ReviewEvent, ReviewEventDto> crudService)
    {
        _mediator = mediator;
        _crudService = crudService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddReviewEntryRequest request, CancellationToken cancellationToken)
    {
        // replace with getting userId from Identity
        // var userId = new Guid(User.FindFirst("Id")!.Value);
        
        var response = await _mediator.Send(request, cancellationToken);
        
        return this.ToActionResult(response);
    }
}