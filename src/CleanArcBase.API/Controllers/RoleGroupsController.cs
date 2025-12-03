using CleanArcBase.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CleanArcBase.Application.Common.Models;
using CleanArcBase.Application.Features.RoleGroups;
using CleanArcBase.Application.Features.RoleGroups.Commands.AssignRolesToGroup;
using CleanArcBase.Application.Features.RoleGroups.Commands.CreateRoleGroup;
using CleanArcBase.Application.Features.RoleGroups.Commands.DeleteRoleGroup;
using CleanArcBase.Application.Features.RoleGroups.Commands.UpdateRoleGroup;
using CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroupById;
using CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroups;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleGroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoleGroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Permission("RoleGroups.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<ActionResult<PagedResponse<RoleGroupDto>>> GetRoleGroups(
        [FromQuery] Guid? tenantId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetRoleGroupsQuery(tenantId, pageNumber, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Permission("RoleGroups.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetRoleGroup(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoleGroupByIdQuery(id), cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Permission("RoleGroups.Create")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> CreateRoleGroup(
        [FromBody] CreateRoleGroupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateRoleGroupCommand(
                request.Name,
                request.Description,
                request.DisplayOrder,
                request.TenantId,
                request.RoleIds),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetRoleGroup), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Permission("RoleGroups.Update")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> UpdateRoleGroup(
        Guid id,
        [FromBody] UpdateRoleGroupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateRoleGroupCommand(
                id,
                request.Name,
                request.Description,
                request.DisplayOrder),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Permission("RoleGroups.Delete")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> DeleteRoleGroup(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteRoleGroupCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPut("{id:guid}/roles")]
    [Permission("RoleGroups.AssignRoles")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> AssignRolesToGroup(
        Guid id,
        [FromBody] AssignRolesToGroupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AssignRolesToGroupCommand(id, request.RoleIds),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}

public record CreateRoleGroupRequest(
    string Name,
    string? Description,
    int DisplayOrder,
    Guid? TenantId,
    List<Guid>? RoleIds);

public record UpdateRoleGroupRequest(
    string Name,
    string? Description,
    int DisplayOrder);

public record AssignRolesToGroupRequest(List<Guid> RoleIds);
