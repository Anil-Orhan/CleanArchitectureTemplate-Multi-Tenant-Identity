using CleanArcBase.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentTenantService _tenantService;

    public UsersController(IIdentityService identityService, ICurrentTenantService tenantService)
    {
        _identityService = identityService;
        _tenantService = tenantService;
    }

    [HttpGet]
    [Permission("Users.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        if (_tenantService.TenantId == null)
            return BadRequest(new { error = "Tenant not identified" });

        var users = await _identityService.GetUsersAsync(_tenantService.TenantId.Value, cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Permission("Users.Read")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(id, cancellationToken);

        if (user == null)
            return NotFound();

        if (_tenantService.TenantId != null && user.TenantId != _tenantService.TenantId)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Permission("Users.Create")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (_tenantService.TenantId == null)
            return BadRequest(new { error = "Tenant not identified" });

        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            _tenantService.TenantId.Value,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (!succeeded)
            return BadRequest(new { errors });

        return CreatedAtAction(nameof(GetUser), new { id = userId }, new { userId });
    }

    [HttpDelete("{id:guid}")]
    [Permission("Users.Delete")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteUserAsync(id, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id:guid}/roles")]
    [Permission("Users.Update")]
    [EnableRateLimiting("Sensitive")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AssignRolesToUserAsync(id, request.Roles, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Roles assigned successfully" });
    }

    [HttpDelete("{id:guid}/roles")]
    [Permission("Users.Update")]
    [EnableRateLimiting("Sensitive")]
    public async Task<IActionResult> RemoveRoles(Guid id, [FromBody] AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RemoveRolesFromUserAsync(id, request.Roles, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Roles removed successfully" });
    }
}

public record CreateUserRequest(string Email, string Password, string FirstName, string LastName);
public record AssignRolesRequest(IEnumerable<string> Roles);
