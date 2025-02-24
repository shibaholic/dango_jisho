using Application.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Utilities;

public static class ResultMapper
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Response<T> response)
        where T : class // ToDo: replace with Dto
    {
        return response.Status switch
        {
            Status.Ok => controller.Ok(response),
            Status.NoContent => controller.NoContent(),
            Status.BadRequest => controller.BadRequest(response.Message),
            Status.Unauthorized => controller.Unauthorized(response.Message),
            Status.NotFound => controller.NotFound(response.Message),
            Status.ServerError => controller.StatusCode(500, response.Message),
            _ => controller.StatusCode(500) // Default fallback
        };
    }
}