using CleanArcBase.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CleanArcBase.Application.Common.Models;
using CleanArcBase.Application.Features.Tenants;
using CleanArcBase.Application.Features.Tenants.Commands.CreateTenant;
using CleanArcBase.Application.Features.Tenants.Queries.GetTenantById;
using CleanArcBase.Application.Features.Tenants.Queries.GetTenants;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Permission("Tenants.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<ActionResult<PagedResponse<TenantDto>>> GetTenants(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetTenantsQuery(pageNumber, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Permission("Tenants.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetTenant(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id), cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Permission("Tenants.Create")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateTenantCommand(request.Name, request.Identifier),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetTenant), new { id = result.Value.Id }, result.Value);
    }
}

public record CreateTenantRequest(string Name, string Identifier);
