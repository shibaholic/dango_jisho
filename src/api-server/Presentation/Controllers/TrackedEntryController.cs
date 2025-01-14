using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Domain.Entities.Tracking;
using Microsoft.AspNetCore.Mvc;
using Application.Response;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class TrackedEntryController: BaseApiController
{
    private readonly ICrudService<TrackedEntry, TrackedEntryDto> _crudService;

    public TrackedEntryController(ICrudService<TrackedEntry, TrackedEntryDto> crudService)
    {
        _crudService = crudService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _crudService.GetAllAsync();

        return this.ToActionResult(response);
    }
}