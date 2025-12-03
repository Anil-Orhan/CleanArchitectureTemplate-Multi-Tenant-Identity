using CleanArcBase.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using CleanArcBase.Application.Features.Roles;
using CleanArcBase.Application.Features.Roles.Commands.AssignPermissions;
using CleanArcBase.Application.Features.Roles.Commands.CreateRole;
using CleanArcBase.Application.Features.Roles.Commands.DeleteRole;
using CleanArcBase.Application.Features.Roles.Commands.UpdateRole;
using CleanArcBase.Application.Features.Roles.Queries.GetRoleById;
using CleanArcBase.Application.Features.Roles.Queries.GetRoles;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentTenantService _tenantService;

    public RolesController(IMediator mediator, ICurrentTenantService tenantService)
    {
        _mediator = mediator;
        _tenantService = tenantService;
    }

    [HttpGet]
    [Permission("Roles.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<ActionResult<PagedResponse<RoleDto>>> GetRoles(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetRolesQuery(_tenantService.TenantId, pageNumber, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Permission("Roles.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id, _tenantService.TenantId), cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Permission("Roles.Create")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateRoleCommand(request.Name, request.Description, _tenantService.TenantId),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetRole), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Permission("Roles.Update")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateRoleCommand(id, request.Name, request.Description, _tenantService.TenantId),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Permission("Roles.Delete")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteRoleCommand(id, _tenantService.TenantId),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPost("{id:guid}/permissions")]
    [Permission("Roles.AssignPermissions")]
    [EnableRateLimiting("Sensitive")]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AssignPermissionsCommand(id, request.PermissionIds, _tenantService.TenantId),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Permissions assigned successfully" });
    }
}

public record CreateRoleRequest(string Name, string? Description);
public record UpdateRoleRequest(string Name, string? Description);
public record AssignPermissionsRequest(IEnumerable<Guid> PermissionIds);
