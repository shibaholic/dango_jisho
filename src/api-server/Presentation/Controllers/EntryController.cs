using Application.Response;
using Application.UseCaseQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Get([FromQuery] EntryQueryRequest request)
    {
        var response = await _mediatr.Send(request);

        if (!response.Successful)
        {
            return Problem(response.Message);
        }

        return Ok(response);
    }
}