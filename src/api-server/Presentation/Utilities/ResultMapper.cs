using Application.Response;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Utilities;

public static class ResultMapper
{
    public static IActionResult ToActionResult<T>(Response<T> response)
        where T : class // ToDo: replace with Dto
    {
        return response.Status switch
        {
            Status.Ok => new OkObjectResult(response.Data),
            Status.NoContent => new NoContentResult(),
            Status.BadRequest => new BadRequestObjectResult(response.Message),
            Status.NotFound => new NotFoundObjectResult(response.Message),
            Status.ServerError => new ObjectResult(response.Message) { StatusCode = 500 },
            _ => new StatusCodeResult(500) // Default fallback
        };
    }
}