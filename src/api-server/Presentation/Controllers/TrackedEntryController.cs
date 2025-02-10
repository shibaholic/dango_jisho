using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Domain.Entities.Tracking;
using Microsoft.AspNetCore.Mvc;
using Application.Response;
using Application.UseCaseQueries;
using MediatR;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class TrackedEntryController: BaseApiController
{
    private readonly ICrudService<TrackedEntry, TrackedEntryDto> _crudService;
    private readonly IMediator _mediator;

    public TrackedEntryController(ICrudService<TrackedEntry, TrackedEntryDto> crudService, IMediator mediator)
    {
        _crudService = crudService;
        _mediator = mediator;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _crudService.GetAllDtoAsync();

        return this.ToActionResult(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByEnt_Seq([FromQuery] string ent_seq, CancellationToken cancellationToken)
    {
        var request = new TrackedEntryIdGetRequest
        {
            ent_seq = ent_seq,
            UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099") // TODO: implement user authentication
        };
        
        var response = await _mediator.Send(request, cancellationToken);
        
        return this.ToActionResult(response);
    }
}