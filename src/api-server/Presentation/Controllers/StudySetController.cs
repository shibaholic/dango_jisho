// using Application.UseCaseCommands;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Presentation.Utilities;
//
// namespace Presentation.Controllers;
//
// [ApiController]
// public class StudySetController : BaseApiController
// {
//     private readonly IMediator _mediator;
//
//     public StudySetController(IMediator mediator)
//     {
//         _mediator = mediator;
//     }
//
//     [HttpPost("start")]
//     public async Task<IActionResult> StartStudySet([FromBody] StartStudySetRequest request, CancellationToken cancellationToken)
//     {
//         var response = await _mediator.Send(request, cancellationToken);
//         
//         return this.ToActionResult(response);
//     }
// }