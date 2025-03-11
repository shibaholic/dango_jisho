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
    private readonly ICrudService<TrackedEntry, TE_EntryDto> _crudService;
    private readonly IMediator _mediator;

    public TrackedEntryController(ICrudService<TrackedEntry, TE_EntryDto> crudService, IMediator mediator)
    {
        _crudService = crudService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _crudService.GetAllDtoAsync();

        return this.ToActionResult(response);
    }
    
    [HttpGet("{ent_seq}")]
    public async Task<IActionResult> GetByEnt_Seq(string ent_seq, CancellationToken cancellationToken)
    {
        var request = new TrackedEntryIdGetRequest
        {
            ent_seq = ent_seq,
            UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099") // TODO: implement user authentication
        };
        
        var response = await _mediator.Send(request, cancellationToken);
        
        return this.ToActionResult(response);
    }
    
    [HttpGet("tag/{tagId}")]
    public async Task<IActionResult> GetByTagId(Guid tagId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);
        if(userId.Equals(Guid.Empty)) return BadRequest();
        
        var request = new TrackedEntriesByTagIdRequest {TagId = tagId, UserId = userId, PageIndex = pageIndex, PageSize = pageSize};
        
        var response = await _mediator.Send(request, cancellationToken);
        
        return this.ToActionResult(response);
    }
}