using Application.Response;
using Application.UseCaseQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class EntryController : ControllerBase
{
    private readonly IMediator _mediatr;

    public EntryController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] EntryQueryRequest request)
    {
        var response = await _mediatr.Send(request);
        
        // ToDo: Also need to implement Automapper and Dtos
        return ResultMapper.ToActionResult(response);
    }
}   