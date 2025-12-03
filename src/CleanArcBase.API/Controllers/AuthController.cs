using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("Public")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (!result.Succeeded)
            return Unauthorized(new { error = result.Error });

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt
        });
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("Public")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!result.Succeeded)
            return Unauthorized(new { error = result.Error });

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt
        });
    }

    [Authorize]
    [HttpPost("revoke")]
    [EnableRateLimiting("Write")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RevokeTokenAsync(request.RefreshToken, cancellationToken);

        if (!result)
            return BadRequest(new { error = "Token revocation failed" });

        return Ok(new { message = "Token revoked successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    [EnableRateLimiting("Standard")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Unauthorized();

        var user = await _identityService.GetUserByIdAsync(userGuid, cancellationToken);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
