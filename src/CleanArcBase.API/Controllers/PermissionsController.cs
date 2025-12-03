using CleanArcBase.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanArcBase.Application.Features.Permissions.Queries.GetAllPermissions;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Permission("Roles.Read")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllPermissionsQuery(), cancellationToken);
        return Ok(result);
    }
}
