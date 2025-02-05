using Application.Mappings.EntityDtos.CardData;
using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseQueries;
using Domain.Entities.CardData;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class CardController : BaseApiController
{
    private readonly IMediator _mediator;

    public CardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] string ent_seq, CancellationToken cancellationToken = default)
    {
        var response = await _mediator.Send(new CardEntSeqGetRequest { ent_seq = ent_seq }, cancellationToken);
        return this.ToActionResult(response); 
    }
}